using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskList.Dtos;
using TaskList.Exceptions;
using TaskList.Interfaces;

namespace TaskList.Features.DeleteTask;

[ApiController]
[Produces("application/json", "application/problem+json")]
[Route("api/tasks")]
[Tags("Task")]
public class DeleteTaskEndpoint(
    IAuthService authService,
    II18nService i18n,
    ITaskRepository taskRepository,
    IUnitOfWork unitOfWork
) : ControllerBase
{
    [Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError,
        Type = typeof(ProblemDetailsDto)
    )]
    public async Task<IActionResult> ExecuteAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var userId = authService.GetAuthenticatedUserId();

        var task = await taskRepository.FindOneAsync(
            x => x.Id == id && x.UserId == userId,
            cancellationToken
        );

        if (task is null)
        {
            throw new NotFoundException(i18n.T("TaskNotFound"));
        }

        taskRepository.Delete(task);

        await unitOfWork.CommitAsync(cancellationToken);

        return NoContent();
    }
}
