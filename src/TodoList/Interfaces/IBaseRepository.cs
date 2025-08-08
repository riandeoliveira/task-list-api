using System.Linq.Expressions;
using TodoList.Dtos;
using TodoList.Entities;

namespace TodoList.Interfaces;

public interface IBaseRepository<TEntity>
    where TEntity : BaseEntity
{
    public Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken);

    public void Delete(TEntity entity);

    public Task DeleteManyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken
    );

    public Task DeleteOneAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken
    );

    public Task<bool> ExistAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken
    );

    public Task<IEnumerable<TEntity>> FindManyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken
    );

    public Task<TEntity?> FindOneAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken
    );

    public Task<PaginationDto<TEntity>> PaginateAsync(
        Expression<Func<TEntity, bool>> predicate,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken
    );

    public void Update(TEntity entity);
}
