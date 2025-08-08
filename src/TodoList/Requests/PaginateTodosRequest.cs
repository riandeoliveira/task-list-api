namespace TodoList.Requests;

public record PaginateTodosRequest(int? PageNumber, int? PageSize);
