using TodoList.Contexts;
using TodoList.Entities;
using TodoList.Interfaces;

namespace TodoList.Repositories;

public class UserRepository(AppDbContext context)
    : BaseRepository<User>(context),
        IUserRepository { }
