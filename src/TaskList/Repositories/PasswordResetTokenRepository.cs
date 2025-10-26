using TaskList.Contexts;
using TaskList.Entities;
using TaskList.Interfaces;

namespace TaskList.Repositories;

public class PasswordResetTokenRepository(AppDbContext context)
    : BaseRepository<PasswordResetToken>(context),
        IPasswordResetTokenRepository { }
