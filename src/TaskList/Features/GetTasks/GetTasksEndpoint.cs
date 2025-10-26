using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskList.Dtos;
using TaskList.Interfaces;
using TaskStatus = TaskList.Enums.TaskStatus;

namespace TaskList.Features.GetTasks;

[ApiController]
[Produces("application/json", "application/problem+json")]
[Route("api/tasks")]
[Tags("Task")]
public class GetTasksEndpoint(IAuthService authService, ITaskRepository taskRepository)
    : ControllerBase
{
    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginationDto<TaskDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError,
        Type = typeof(ProblemDetailsDto)
    )]
    public async Task<IActionResult> ExecuteAsync(
        [FromQuery(Name = "page_number")] int? PageNumber,
        [FromQuery(Name = "page_size")] int? PageSize,
        [FromQuery(Name = "status")] TaskStatus? Status,
        CancellationToken cancellationToken
    )
    {
        var userId = authService.GetAuthenticatedUserId();

        var paginatedTasks = await taskRepository.PaginateAsync(
            x =>
                x.UserId == userId
                && (
                    !Status.HasValue
                    || (Status == TaskStatus.Completed && x.IsCompleted)
                    || (Status == TaskStatus.Pending && !x.IsCompleted)
                ),
            PageNumber,
            PageSize,
            cancellationToken
        );

        var tasks = paginatedTasks.Items.Select(x => new TaskDto(
            x.Id,
            x.Title,
            x.Description,
            x.IsCompleted,
            x.CreatedAt
        ));

        var response = new PaginationDto<TaskDto>(
            paginatedTasks.PageNumber,
            paginatedTasks.PageSize,
            paginatedTasks.TotalItems,
            tasks
        );

        return Ok(response);
    }
}
