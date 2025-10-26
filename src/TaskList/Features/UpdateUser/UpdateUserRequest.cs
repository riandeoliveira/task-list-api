namespace TaskList.Features.UpdateUser;

public record UpdateUserRequest(
    string? Name,
    string? Username,
    string? Email,
    string? Password,
    string? NewPassword,
    string? NewPasswordConfirmation
);
