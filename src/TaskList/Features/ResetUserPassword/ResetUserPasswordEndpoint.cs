using Microsoft.AspNetCore.Mvc;
using TaskList.Dtos;
using TaskList.Entities;
using TaskList.Exceptions;
using TaskList.Interfaces;

namespace TaskList.Features.ResetUserPassword;

[ApiController]
[Produces("application/json", "application/problem+json")]
[Route("api/users")]
[Tags("User")]
public class ResetUserPasswordEndpoint(
    IAuthService authService,
    II18nService i18n,
    IPasswordResetTokenRepository passwordResetTokenRepository,
    IPasswordService passwordService,
    IPersonalRefreshTokenRepository personalRefreshTokenRepository,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository
) : ControllerBase
{
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError,
        Type = typeof(ProblemDetailsDto)
    )]
    public async Task<IActionResult> ExecuteAsync(
        [FromBody] ResetUserPasswordRequest request,
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

        var hashedPassword = passwordService.HashPassword(request.Password.Trim());

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

        return NoContent();
    }
}
