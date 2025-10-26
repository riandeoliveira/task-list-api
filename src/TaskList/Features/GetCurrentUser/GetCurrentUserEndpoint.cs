using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskList.Dtos;
using TaskList.Exceptions;
using TaskList.Interfaces;

namespace TaskList.Features.GetCurrentUser;

[ApiController]
[Produces("application/json", "application/problem+json")]
[Route("api/users")]
[Tags("User")]
public class GetCurrentUserEndpoint(
    IAuthService authService,
    IUserRepository userRepository,
    II18nService i18n
) : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError,
        Type = typeof(ProblemDetailsDto)
    )]
    public async Task<IActionResult> ExecuteAsync(CancellationToken cancellationToken)
    {
        var userId = authService.GetAuthenticatedUserId();
        var user = await userRepository.FindOneAsync(x => x.Id == userId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException(i18n.T("UserNotFound"));
        }

        var response = new UserDto(user.Id, user.Name, user.Username, user.Email, user.CreatedAt);

        return Ok(response);
    }
}
