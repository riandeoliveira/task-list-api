using TodoList.Constants;
using TodoList.Dtos;
using TodoList.Entities;
using TodoList.Exceptions;
using TodoList.Interfaces;
using TodoList.Requests;
using TodoList.Utils;
using TodoList.ViewModels;

namespace TodoList.Handlers;

public class UserHandler(
    IAuthService authService,
    II18nService i18n,
    IMailService mailService,
    IPasswordResetTokenRepository passwordResetTokenRepository,
    IPersonalRefreshTokenRepository personalRefreshTokenRepository,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository
) : IUserHandler
{
    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
        var userId = authService.GetAuthenticatedUserId();

        await userRepository.DeleteOneAsync(x => x.Id == userId, cancellationToken);

        await personalRefreshTokenRepository.DeleteManyAsync(
            x => x.UserId == userId,
            cancellationToken
        );

        await passwordResetTokenRepository.DeleteManyAsync(
            x => x.UserId == userId,
            cancellationToken
        );

        await unitOfWork.CommitAsync(cancellationToken);

        authService.ClearAuthCookiesFromClient();
    }

    public async Task ForgotPasswordAsync(
        ForgotUserPasswordRequest request,
        CancellationToken cancellationToken
    )
    {
        var usernameOrEmail = request.UsernameOrEmail.Trim();

        var user = await userRepository.FindOneAsync(
            x => x.Username == usernameOrEmail || x.Email == usernameOrEmail,
            cancellationToken
        );

        if (user is null)
        {
            return;
        }

        var token = authService.GenerateJwtToken(user.Id, DateTime.UtcNow.AddMinutes(30));

        var passwordResetToken = new PasswordResetToken
        {
            Value = token.Value,
            ExpiresAt = token.ExpiresAt,
            UserId = user.Id,
        };

        await passwordResetTokenRepository.CreateAsync(passwordResetToken, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        var resetPasswordUrl =
            $"{EnvironmentVariables.ClientUrl}/reset-password?token={token.Value}";

        var viewModel = new ForgotUserPasswordViewModel(user.Name, resetPasswordUrl);

        var mailSender = new MailSenderDto(
            Recipient: user.Email,
            Subject: i18n.T("PasswordResetRequest"),
            View: new ViewDto("ForgotUserPassword", viewModel)
        );

        await mailService.SendMailAsync(mailSender, cancellationToken);
    }

    public async Task RenewRefreshTokenAsync(CancellationToken cancellationToken)
    {
        var currentRefreshToken = authService.GetRefreshTokenFromCookies();

        var storeRefreshToken = await personalRefreshTokenRepository.FindOneAsync(
            x => x.Value == currentRefreshToken,
            cancellationToken
        );

        if (storeRefreshToken is null)
        {
            throw new UnauthorizedException(i18n.T("UnauthorizedOperation"));
        }

        var user = await userRepository.FindOneAsync(
            x => x.Id == storeRefreshToken.UserId,
            cancellationToken
        );

        if (user is null)
        {
            throw new NotFoundException(i18n.T("UserNotFound"));
        }

        await personalRefreshTokenRepository.DeleteOneAsync(
            x => x.Id == storeRefreshToken.Id,
            cancellationToken
        );

        var authTokens = authService.GenerateAuthTokens(user.Id);

        var personalRefreshToken = new PersonalRefreshToken
        {
            Value = authTokens.RefreshToken.Value,
            ExpiresAt = authTokens.RefreshToken.ExpiresAt,
            UserId = user.Id,
        };

        await personalRefreshTokenRepository.CreateAsync(personalRefreshToken, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        authService.SendAuthCookiesToClient(authTokens);
    }

    public async Task ResetPasswordAsync(
        ResetUserPasswordRequest request,
        CancellationToken cancellationToken
    )
    {
        var passwordResetToken = await passwordResetTokenRepository.FindOneAsync(
            x => x.Value == request.Token && x.ExpiresAt > DateTime.UtcNow,
            cancellationToken
        );

        if (passwordResetToken is null)
        {
            throw new UnauthorizedException(i18n.T("UnauthorizedOperation"));
        }

        var user = await userRepository.FindOneAsync(
            x => x.Id == passwordResetToken.UserId,
            cancellationToken
        );

        if (user is null)
        {
            throw new NotFoundException(i18n.T("UserNotFound"));
        }

        var hashedPassword = SecurityUtil.HashPassword(request.Password.Trim());

        user.Password = hashedPassword;

        userRepository.Update(user);

        await passwordResetTokenRepository.DeleteManyAsync(
            x => x.UserId == user.Id,
            cancellationToken
        );

        var authTokens = authService.GenerateAuthTokens(user.Id);

        var personalRefreshToken = new PersonalRefreshToken
        {
            Value = authTokens.RefreshToken.Value,
            ExpiresAt = authTokens.RefreshToken.ExpiresAt,
            UserId = user.Id,
        };

        await personalRefreshTokenRepository.CreateAsync(personalRefreshToken, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        authService.SendAuthCookiesToClient(authTokens);
    }

    public async Task SignInAsync(SignInUserRequest request, CancellationToken cancellationToken)
    {
        var usernameOrEmail = request.UsernameOrEmail.Trim();

        var user = await userRepository.FindOneAsync(
            x => x.Username == usernameOrEmail || x.Email == usernameOrEmail,
            cancellationToken
        );

        if (user is null || !SecurityUtil.VerifyPassword(request.Password.Trim(), user.Password))
        {
            throw new UnauthorizedException(i18n.T("InvalidCredentials"));
        }

        var currentRefreshToken = authService.GetRefreshTokenFromCookies();

        await personalRefreshTokenRepository.DeleteOneAsync(
            x => x.Value == currentRefreshToken,
            cancellationToken
        );

        var authTokens = authService.GenerateAuthTokens(user.Id);

        var personalRefreshToken = new PersonalRefreshToken
        {
            Value = authTokens.RefreshToken.Value,
            ExpiresAt = authTokens.RefreshToken.ExpiresAt,
            UserId = user.Id,
        };

        await personalRefreshTokenRepository.CreateAsync(personalRefreshToken, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        authService.SendAuthCookiesToClient(authTokens);
    }

    public async Task SignOutAsync(CancellationToken cancellationToken)
    {
        var currentRefreshToken = authService.GetRefreshTokenFromCookies();

        await personalRefreshTokenRepository.DeleteOneAsync(
            x => x.Value == currentRefreshToken,
            cancellationToken
        );

        await unitOfWork.CommitAsync(cancellationToken);

        authService.ClearAuthCookiesFromClient();
    }

    public async Task<UserDto> SignUpAsync(
        SignUpUserRequest request,
        CancellationToken cancellationToken
    )
    {
        var name = request.Name.Trim();
        var username = request.Username.Trim();
        var email = request.Email.Trim();

        var userAlreadyExists = await userRepository.ExistAsync(
            x => x.Username == username || x.Email == email,
            cancellationToken
        );

        if (userAlreadyExists)
        {
            throw new ConflictException(i18n.T("UserAlreadyExists"));
        }

        var hashedPassword = SecurityUtil.HashPassword(request.Password.Trim());

        var user = new User
        {
            Name = name,
            Username = username,
            Email = email,
            Password = hashedPassword,
        };

        await userRepository.CreateAsync(user, cancellationToken);

        var authTokens = authService.GenerateAuthTokens(user.Id);

        var personalRefreshToken = new PersonalRefreshToken()
        {
            Value = authTokens.RefreshToken.Value,
            ExpiresAt = authTokens.RefreshToken.ExpiresAt,
            UserId = user.Id,
        };

        await personalRefreshTokenRepository.CreateAsync(personalRefreshToken, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        authService.SendAuthCookiesToClient(authTokens);

        return new UserDto(user.Id, user.Name, user.Username, user.Email, user.CreatedAt);
    }

    public async Task<UserDto> UpdateAsync(
        UpdateUserRequest request,
        CancellationToken cancellationToken
    )
    {
        var userId = authService.GetAuthenticatedUserId();
        var user = await userRepository.FindOneAsync(x => x.Id == userId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException(i18n.T("UserNotFound"));
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            user.Name = request.Name.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Username))
        {
            var username = request.Username.Trim();

            var usernameAlreadyExists = await userRepository.ExistAsync(
                x => x.Id != userId && x.Username == username,
                cancellationToken
            );

            if (usernameAlreadyExists)
            {
                throw new ConflictException(i18n.T("UserAlreadyExists"));
            }

            user.Username = username;
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var email = request.Email.Trim();

            var emailAlreadyExists = await userRepository.ExistAsync(
                x => x.Id != userId && x.Email == email,
                cancellationToken
            );

            if (emailAlreadyExists)
            {
                throw new ConflictException(i18n.T("UserAlreadyExists"));
            }

            user.Email = email;
        }

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            var hashedPassword = SecurityUtil.HashPassword(request.Password.Trim());

            user.Password = hashedPassword;

            var currentRefreshToken = authService.GetRefreshTokenFromCookies();

            await personalRefreshTokenRepository.DeleteOneAsync(
                x => x.Value == currentRefreshToken,
                cancellationToken
            );

            var authTokens = authService.GenerateAuthTokens(user.Id);

            var personalRefreshToken = new PersonalRefreshToken
            {
                Value = authTokens.RefreshToken.Value,
                ExpiresAt = authTokens.RefreshToken.ExpiresAt,
                UserId = user.Id,
            };

            await personalRefreshTokenRepository.CreateAsync(
                personalRefreshToken,
                cancellationToken
            );

            await unitOfWork.CommitAsync(cancellationToken);

            authService.SendAuthCookiesToClient(authTokens);
        }

        userRepository.Update(user);

        await unitOfWork.CommitAsync(cancellationToken);

        return new UserDto(user.Id, user.Name, user.Username, user.Email, user.CreatedAt);
    }
}
