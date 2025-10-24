namespace SharedKernel.Abstractions;

public interface IDatabase
{
    Task<IEnumerable<TEntity>> GetAsync<TEntity>(CancellationToken cancellationToken = default)
        where TEntity : class;

    Task AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class;
}
