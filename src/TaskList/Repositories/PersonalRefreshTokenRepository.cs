using TaskList.Contexts;
using TaskList.Entities;
using TaskList.Interfaces;

namespace TaskList.Repositories;

public class PersonalRefreshTokenRepository(AppDbContext context)
    : BaseRepository<PersonalRefreshToken>(context),
        IPersonalRefreshTokenRepository { }
