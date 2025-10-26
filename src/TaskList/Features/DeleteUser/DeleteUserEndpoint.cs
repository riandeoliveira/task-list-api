using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskList.Dtos;
using TaskList.Interfaces;

namespace TaskList.Features.DeleteUser;

[ApiController]
[Produces("application/json", "application/problem+json")]
[Route("api/users")]
[Tags("User")]
public class DeleteUserEndpoint(
    IAuthService authService,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository
) : ControllerBase
{
    [Authorize]
    [HttpDelete("me")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError,
        Type = typeof(ProblemDetailsDto)
    )]
    public async Task<IActionResult> ExecuteAsync(CancellationToken cancellationToken)
    {
        var userId = authService.GetAuthenticatedUserId();

        await userRepository.DeleteAsync(x => x.Id == userId, cancellationToken);

        authService.ClearAuthCookiesFromClient();

        await unitOfWork.CommitAsync(cancellationToken);

        return NoContent();
    }
}
