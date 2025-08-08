namespace TodoList.Requests;

public record UpdateUserRequest(string? Name, string? Username, string? Email, string? Password);
