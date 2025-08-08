namespace TodoList.Requests;

public record ResetUserPasswordRequest(string Password, string PasswordConfirmation, string Token);
