using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskList.Dtos;
using TaskList.Exceptions;
using TaskList.Interfaces;
using TaskList.Services;

namespace TaskList.Features.GetTaskById;

[ApiController]
[Produces("application/json", "application/problem+json")]
[Route("api/tasks")]
[Tags("Task")]
public class GetTaskByIdEndpoint(
    IAuthService authService,
    II18nService i18n,
    ITaskRepository taskRepository,
    IUnitOfWork unitOfWork
) : ControllerBase
{
    [Authorize]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TaskDto))]
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

        await unitOfWork.CommitAsync(cancellationToken);

        var response = new TaskDto(
            task.Id,
            task.Title,
            task.Description,
            task.IsCompleted,
            task.CreatedAt
        );

        return Ok(response);
    }
}
