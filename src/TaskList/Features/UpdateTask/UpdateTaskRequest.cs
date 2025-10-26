namespace TaskList.Features.UpdateTask;

public record UpdateTaskRequest(string? Title, string? Description, bool? IsCompleted);
