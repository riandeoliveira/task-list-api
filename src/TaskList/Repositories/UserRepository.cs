using TaskList.Contexts;
using TaskList.Entities;
using TaskList.Interfaces;

namespace TaskList.Repositories;

public class UserRepository(AppDbContext context)
    : BaseRepository<User>(context),
        IUserRepository { }
