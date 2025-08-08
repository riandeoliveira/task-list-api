using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.Dtos;
using TodoList.Interfaces;
using TodoList.Requests;

namespace TodoList.Controllers;

[ApiController]
[Produces("application/json", "application/problem+json")]
[Route("api/todos")]
public class TodoController(ITodoHandler handler) : ControllerBase
{
    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError,
        Type = typeof(ProblemDetailsDto)
    )]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateTodoRequest request,
        CancellationToken cancellationToken
    )
    {
        await handler.CreateAsync(request, cancellationToken);

        return StatusCode(StatusCodes.Status201Created);
    }

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
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        await handler.DeleteAsync(id, cancellationToken);

        return NoContent();
    }

    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError,
        Type = typeof(ProblemDetailsDto)
    )]
    public async Task<IActionResult> PaginateAsync(
        [FromQuery(Name = "page_number")] int? PageNumber,
        [FromQuery(Name = "page_size")] int? PageSize,
        CancellationToken cancellationToken
    )
    {
        var request = new PaginateTodosRequest(PageNumber, PageSize);
        var response = await handler.PaginateAsync(request, cancellationToken);

        return Ok(response);
    }

    [Authorize]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError,
        Type = typeof(ProblemDetailsDto)
    )]
    public async Task<IActionResult> FindOneAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var response = await handler.FindOneAsync(id, cancellationToken);

        return Ok(response);
    }

    [Authorize]
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError,
        Type = typeof(ProblemDetailsDto)
    )]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateTodoRequest request,
        CancellationToken cancellationToken
    )
    {
        await handler.UpdateAsync(id, request, cancellationToken);

        return NoContent();
    }
}
