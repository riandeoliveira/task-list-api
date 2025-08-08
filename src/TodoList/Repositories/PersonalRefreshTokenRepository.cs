using TodoList.Contexts;
using TodoList.Entities;
using TodoList.Interfaces;

namespace TodoList.Repositories;

public class PersonalRefreshTokenRepository(AppDbContext context)
    : BaseRepository<PersonalRefreshToken>(context),
        IPersonalRefreshTokenRepository { }
