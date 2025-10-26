using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskList.Dtos;
using TaskList.Exceptions;
using TaskList.Interfaces;

namespace TaskList.Features.UpdateTask;

[ApiController]
[Produces("application/json", "application/problem+json")]
[Route("api/tasks")]
[Tags("Task")]
public class UpdateTaskEndpoint(
    IAuthService authService,
    II18nService i18n,
    ITaskRepository taskRepository,
    IUnitOfWork unitOfWork
) : ControllerBase
{
    [Authorize]
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TaskDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError,
        Type = typeof(ProblemDetailsDto)
    )]
    public async Task<IActionResult> ExecuteAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateTaskRequest request,
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

        task.Title = request.Title?.Trim() ?? task.Title;
        task.Description = request.Description?.Trim() ?? task.Description;
        task.IsCompleted = request.IsCompleted ?? task.IsCompleted;

        taskRepository.Update(task);

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
