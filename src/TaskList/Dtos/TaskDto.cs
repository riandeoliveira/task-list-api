namespace TaskList.Dtos;

public record TaskDto(
    Guid Id,
    string Title,
    string Description,
    bool IsCompleted,
    DateTime CreatedAt
);
