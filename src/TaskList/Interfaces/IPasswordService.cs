namespace TaskList.Interfaces;

public interface IPasswordService
{
    public string HashPassword(string password);

    public bool VerifyPassword(string hashedPassword, string providedPassword);
}
