namespace TaskList.Features.SignUpUser;

public record SignUpUserRequest(
    string Name,
    string Username,
    string Email,
    string Password,
    string PasswordConfirmation
);
