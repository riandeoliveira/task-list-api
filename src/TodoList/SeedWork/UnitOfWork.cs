using TodoList.Contexts;
using TodoList.Interfaces;

namespace TodoList.SeedWork;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
