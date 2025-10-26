using Microsoft.AspNetCore.Mvc;
using TaskList.Dtos;
using TaskList.Entities;
using TaskList.Exceptions;
using TaskList.Interfaces;

namespace TaskList.Features.SignInUser;

[ApiController]
[Produces("application/json", "application/problem+json")]
[Route("api/users")]
[Tags("User")]
public class SignInUserEndpoint(
    IAuthService authService,
    II18nService i18n,
    IPasswordService passwordService,
    IPersonalRefreshTokenRepository personalRefreshTokenRepository,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository
) : ControllerBase
{
    [HttpPost("sign-in")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError,
        Type = typeof(ProblemDetailsDto)
    )]
    public async Task<IActionResult> ExecuteAsync(
        [FromBody] SignInUserRequest request,
        CancellationToken cancellationToken
    )
    {
        var usernameOrEmail = request.UsernameOrEmail.Trim();

        var user = await userRepository.FindOneAsync(
            x => x.Username == usernameOrEmail || x.Email == usernameOrEmail,
            cancellationToken
        );

        if (user is null || !passwordService.VerifyPassword(user.Password, request.Password.Trim()))
        {
            throw new UnauthorizedException(i18n.T("InvalidCredentials"));
        }

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

        await personalRefreshTokenRepository.CreateAsync(personalRefreshToken, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        authService.SendAuthCookiesToClient(authTokens);

        return NoContent();
    }
}
