using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskList.Dtos;
using TaskList.Entities;
using TaskList.Exceptions;
using TaskList.Interfaces;

namespace TaskList.Features.UpdateUser;

[ApiController]
[Produces("application/json", "application/problem+json")]
[Route("api/users")]
[Tags("User")]
public class UpdateUserEndpoint(
    IAuthService authService,
    II18nService i18n,
    IPasswordService passwordService,
    IPersonalRefreshTokenRepository personalRefreshTokenRepository,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository
) : ControllerBase
{
    [Authorize]
    [HttpPatch("me")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError,
        Type = typeof(ProblemDetailsDto)
    )]
    public async Task<IActionResult> ExecuteAsync(
        [FromBody] UpdateUserRequest request,
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

        if (
            !string.IsNullOrWhiteSpace(request.Password)
            && !string.IsNullOrWhiteSpace(request.NewPassword)
        )
        {
            var isCorrectPassword = passwordService.VerifyPassword(
                user.Password,
                request.Password.Trim()
            );

            if (!isCorrectPassword)
            {
                throw new BadRequestException(i18n.T("IncorrectCurrentPassword"));
            }

            var hashedPassword = passwordService.HashPassword(request.NewPassword.Trim());

            user.Password = hashedPassword;

            var currentRefreshToken = authService.GetRefreshTokenFromCookies();

            await personalRefreshTokenRepository.DeleteAsync(
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

            authService.SendAuthCookiesToClient(authTokens);
        }

        userRepository.Update(user);

        await unitOfWork.CommitAsync(cancellationToken);

        var response = new UserDto(user.Id, user.Name, user.Username, user.Email, user.CreatedAt);

        return Ok(response);
    }
}
