using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskList.Dtos;
using TaskList.Entities;
using TaskList.Exceptions;
using TaskList.Interfaces;

namespace TaskList.Features.RenewUserRefreshToken;

[ApiController]
[Produces("application/json", "application/problem+json")]
[Route("api/users")]
[Tags("User")]
public class RenewUserRefreshTokenEndpoint(
    IAuthService authService,
    II18nService i18n,
    IPersonalRefreshTokenRepository personalRefreshTokenRepository,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository
) : ControllerBase
{
    [HttpPost("renew-refresh-token")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError,
        Type = typeof(ProblemDetailsDto)
    )]
    public async Task<IActionResult> ExecuteAsync(CancellationToken cancellationToken)
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
