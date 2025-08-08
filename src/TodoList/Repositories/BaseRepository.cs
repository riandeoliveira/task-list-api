using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TodoList.Contexts;
using TodoList.Dtos;
using TodoList.Entities;
using TodoList.Interfaces;

namespace TodoList.Repositories;

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

    public async Task DeleteManyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken
    )
    {
        var entities = await context.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken);

        context.Set<TEntity>().RemoveRange(entities);
    }

    public async Task DeleteOneAsync(
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
            .OrderBy(x => x.Id)
            .Skip((currentPage - 1) * currentSize)
            .Take(currentSize)
            .ToListAsync(cancellationToken);

        return new PaginationDto<TEntity>(currentPage, currentSize, totalItems, items);

        // const int maxPageSize = 1000;

        // var currentPage = Math.Max(pageNumber, 1);
        // var currentSize = Math.Clamp(pageSize, 1, maxPageSize);

        // var query = context.Set<TEntity>().Where(predicate).AsQueryable();

        // var totalItems = await query.CountAsync(cancellationToken);
        // var totalPages = (int)Math.Ceiling(totalItems / (double)currentSize);

        // long skipLong = (long)(currentPage - 1) * currentSize;
        // int skip = skipLong > int.MaxValue ? int.MaxValue : (int)skipLong;

        // var items = await query.Skip(skip).Take(currentSize).ToListAsync(cancellationToken);

        // return new PaginationDto<TEntity>(currentPage, currentSize, totalItems, totalPages, items);
    }

    public void Update(TEntity entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;

        context.Set<TEntity>().Update(entity);
    }
}
