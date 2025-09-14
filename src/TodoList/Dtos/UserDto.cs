namespace TodoList.Dtos;

public record UserDto(Guid Id, string Name, string Username, string Email, DateTime CreatedAt);
