namespace BrewBoxApi.Domain.SeedWork;

public interface IBaseCommandRepository<T> where T : BaseModel
{
    /// <summary>
    /// Adds a new entity.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>The added entity.</returns>
    ValueTask<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>The updated entity.</returns>
    ValueTask<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft-deletes an entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>A task representing the operation.</returns>
    ValueTask DeleteAsync(string id, CancellationToken cancellationToken = default);
}