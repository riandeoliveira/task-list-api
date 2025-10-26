namespace TaskList.Features.ResetUserPassword;

public record ResetUserPasswordRequest(string Password, string PasswordConfirmation, string Token);
