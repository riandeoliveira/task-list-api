using TaskList.Contexts;
using TaskList.Interfaces;

namespace TaskList.SeedWork;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
