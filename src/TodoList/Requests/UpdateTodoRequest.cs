namespace TodoList.Requests;

public record UpdateTodoRequest(string? Title, string? Description, bool? IsCompleted);
