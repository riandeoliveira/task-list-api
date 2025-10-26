using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskList.Dtos;
using TaskList.Interfaces;
using Task = TaskList.Entities.Task;

namespace TaskList.Features.CreateTask;

[ApiController]
[Produces("application/json", "application/problem+json")]
[Route("api/tasks")]
[Tags("Task")]
public class CreateTaskEndpoint(
    IAuthService authService,
    ITaskRepository taskRepository,
    IUnitOfWork unitOfWork
) : ControllerBase
{
    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TaskDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError,
        Type = typeof(ProblemDetailsDto)
    )]
    public async Task<IActionResult> ExecuteAsync(
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken
    )
    {
        var userId = authService.GetAuthenticatedUserId();

        var task = new Task
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim() ?? "",
            UserId = userId,
        };

        await taskRepository.CreateAsync(task, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        var response = new TaskDto(
            task.Id,
            task.Title,
            task.Description,
            task.IsCompleted,
            task.CreatedAt
        );

        return StatusCode(StatusCodes.Status201Created, response);
    }
}
