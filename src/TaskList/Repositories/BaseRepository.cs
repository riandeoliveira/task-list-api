using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskList.Contexts;
using TaskList.Dtos;
using TaskList.Interfaces;
using BaseEntity = TaskList.Entities.BaseEntity;

namespace TaskList.Repositories;

public abstract class BaseRepository<TEntity>(AppDbContext context) : IBaseRepository<TEntity>
    where TEntity : BaseEntity
{
    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await context.Set<TEntity>().AddAsync(entity, cancellationToken);

        return entity;
    }

    public void Delete(TEntity entity)
    {
        context.Set<TEntity>().Remove(entity);
    }

    public async Task DeleteAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken
    )
    {
        var entity = await context.Set<TEntity>().FirstOrDefaultAsync(predicate, cancellationToken);

        if (entity is not null)
        {
            context.Set<TEntity>().Remove(entity);
        }
    }

    public async Task DeleteManyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken
    )
    {
        var entities = await context.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken);

        context.Set<TEntity>().RemoveRange(entities);
    }

    public async Task<bool> ExistAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken
    )
    {
        var entity = await context
            .Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(predicate, cancellationToken);

        return entity is not null;
    }

    public async Task<IEnumerable<TEntity>> FindManyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken
    )
    {
        var entities = await context
            .Set<TEntity>()
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync(cancellationToken);

        return entities;
    }

    public async Task<TEntity?> FindOneAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken
    )
    {
        var entity = await context
            .Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(predicate, cancellationToken);

        return entity;
    }

    public async Task<PaginationDto<TEntity>> PaginateAsync(
        Expression<Func<TEntity, bool>> predicate,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken
    )
    {
        var currentPage = pageNumber is > 0 and <= 500 ? pageNumber.Value : 1;
        var currentSize = pageSize is > 0 and <= 500 ? pageSize.Value : 10;

        var query = context.Set<TEntity>().Where(predicate).AsQueryable();
        var totalItems = await query.AsNoTracking().CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((currentPage - 1) * currentSize)
            .Take(currentSize)
            .ToListAsync(cancellationToken);

        return new PaginationDto<TEntity>(currentPage, currentSize, totalItems, items);
    }

    public void Update(TEntity entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;

        context.Set<TEntity>().Update(entity);
    }
}
