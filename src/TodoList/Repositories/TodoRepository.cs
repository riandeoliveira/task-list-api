using TodoList.Contexts;
using TodoList.Entities;
using TodoList.Interfaces;

namespace TodoList.Repositories;

public class TodoRepository(AppDbContext context)
    : BaseRepository<Todo>(context),
        ITodoRepository { }
