using Microsoft.AspNetCore.Identity;
using TaskList.Interfaces;

namespace TaskList.Services;

public class PasswordService : IPasswordService
{
    private static readonly object _user = new();

    private readonly PasswordHasher<object> _passwordHasher = new();

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(_user, password);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(_user, hashedPassword, providedPassword);

        return result == PasswordVerificationResult.Success;
    }
}
