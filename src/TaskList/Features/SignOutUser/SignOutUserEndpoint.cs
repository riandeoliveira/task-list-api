using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskList.Dtos;
using TaskList.Interfaces;

namespace TaskList.Features.SignOutUser;

[ApiController]
[Produces("application/json", "application/problem+json")]
[Route("api/users")]
[Tags("User")]
public class SignOutUserEndpoint(
    IAuthService authService,
    IPersonalRefreshTokenRepository personalRefreshTokenRepository,
    IUnitOfWork unitOfWork
) : ControllerBase
{
    [Authorize]
    [HttpPost("sign-out")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError,
        Type = typeof(ProblemDetailsDto)
    )]
    public async Task<IActionResult> ExecuteAsync(CancellationToken cancellationToken)
    {
        var currentRefreshToken = authService.GetRefreshTokenFromCookies();

        await personalRefreshTokenRepository.DeleteAsync(
            x => x.Value == currentRefreshToken,
            cancellationToken
        );

        await unitOfWork.CommitAsync(cancellationToken);

        authService.ClearAuthCookiesFromClient();

        return NoContent();
    }
}
