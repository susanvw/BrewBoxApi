using BrewBoxApi.Domain.SeedWork;

namespace BrewBoxApi.Domain.Aggregates.Drinks;

public interface IDrinkRepository : IBaseReadRepository<Drink>, IBaseCommandRepository<Drink>
{
    /// <summary>
    /// Get price for a specific type and size (lookup).
    /// </summary>
    ValueTask<Drink?> GetByTypeAndSizeAsync(DrinkType type, DrinkSize size, CancellationToken cancellationToken = default);
}