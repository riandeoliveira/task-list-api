using TodoList.Dtos;
using TodoList.Entities;
using TodoList.Exceptions;
using TodoList.Interfaces;
using TodoList.Requests;

namespace TodoList.Handlers;

public class TodoHandler(
    IAuthService authService,
    II18nService i18n,
    ITodoRepository todoRepository,
    IUnitOfWork unitOfWork
) : ITodoHandler
{
    public async Task CreateAsync(CreateTodoRequest request, CancellationToken cancellationToken)
    {
        var userId = authService.GetAuthenticatedUserId();

        var todo = new Todo
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim() ?? "",
            UserId = userId,
        };

        await todoRepository.CreateAsync(todo, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var userId = authService.GetAuthenticatedUserId();

        var todo = await todoRepository.FindOneAsync(
            x => x.Id == id && x.UserId == userId,
            cancellationToken
        );

        if (todo is null)
        {
            throw new NotFoundException(i18n.T("TodoNotFound"));
        }

        todoRepository.Delete(todo);

        await unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task<TodoDto> FindOneAsync(Guid id, CancellationToken cancellationToken)
    {
        var userId = authService.GetAuthenticatedUserId();

        var todo = await todoRepository.FindOneAsync(
            x => x.Id == id && x.UserId == userId,
            cancellationToken
        );

        if (todo is null)
        {
            throw new NotFoundException(i18n.T("TodoNotFound"));
        }

        return new TodoDto(todo.Id, todo.Title, todo.Description, todo.IsCompleted, todo.CreatedAt);
    }

    public async Task<PaginationDto<TodoDto>> PaginateAsync(
        PaginateTodosRequest request,
        CancellationToken cancellationToken
    )
    {
        var userId = authService.GetAuthenticatedUserId();

        var paginatedTodos = await todoRepository.PaginateAsync(
            x => x.UserId == userId,
            request.PageNumber,
            request.PageSize,
            cancellationToken
        );

        var todos = paginatedTodos.Items.Select(x => new TodoDto(
            x.Id,
            x.Title,
            x.Description,
            x.IsCompleted,
            x.CreatedAt
        ));

        return new PaginationDto<TodoDto>(
            paginatedTodos.PageNumber,
            paginatedTodos.PageSize,
            paginatedTodos.TotalItems,
            todos
        );
    }

    public async Task UpdateAsync(
        Guid id,
        UpdateTodoRequest request,
        CancellationToken cancellationToken
    )
    {
        var userId = authService.GetAuthenticatedUserId();

        var todo = await todoRepository.FindOneAsync(
            x => x.Id == id && x.UserId == userId,
            cancellationToken
        );

        if (todo is null)
        {
            throw new NotFoundException(i18n.T("TodoNotFound"));
        }

        todo.Title = request.Title?.Trim() ?? todo.Title;
        todo.Description = request.Description?.Trim() ?? todo.Description;
        todo.IsCompleted = request.IsCompleted ?? todo.IsCompleted;

        todoRepository.Update(todo);

        await unitOfWork.CommitAsync(cancellationToken);
    }
}
