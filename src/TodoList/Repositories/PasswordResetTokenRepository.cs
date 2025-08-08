using TodoList.Contexts;
using TodoList.Entities;
using TodoList.Interfaces;

namespace TodoList.Repositories;

public class PasswordResetTokenRepository(AppDbContext context)
    : BaseRepository<PasswordResetToken>(context),
        IPasswordResetTokenRepository { }
