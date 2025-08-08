using TodoList.Dtos;
using TodoList.Requests;

namespace TodoList.Interfaces;

public interface ITodoHandler
{
    public Task CreateAsync(CreateTodoRequest request, CancellationToken cancellationToken);

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    public Task<TodoDto> FindOneAsync(Guid id, CancellationToken cancellationToken);

    public Task<PaginationDto<TodoDto>> PaginateAsync(
        PaginateTodosRequest request,
        CancellationToken cancellationToken
    );

    public Task UpdateAsync(
        Guid id,
        UpdateTodoRequest request,
        CancellationToken cancellationToken
    );
}
