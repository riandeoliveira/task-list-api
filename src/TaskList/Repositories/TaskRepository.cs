using TaskList.Contexts;
using TaskList.Interfaces;
using Task = TaskList.Entities.Task;

namespace TaskList.Repositories;

public class TaskRepository(AppDbContext context)
    : BaseRepository<Task>(context),
        ITaskRepository { }
