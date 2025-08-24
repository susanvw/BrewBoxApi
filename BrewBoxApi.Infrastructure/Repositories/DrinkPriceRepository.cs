using Microsoft.EntityFrameworkCore;
using BrewBoxApi.Infrastructure.Data;
using BrewBoxApi.Domain.Aggregates.Drinks;

namespace BrewBoxApi.Infrastructure.Repositories;

public class DrinkPriceRepository(ApplicationDbContext context) : BaseRepository<Drink>(context), IDrinkRepository
{
    public async ValueTask<Drink?> GetByTypeAndSizeAsync(DrinkType type, DrinkSize size, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Drink>()
            .FirstOrDefaultAsync(dp => dp.Type == type && dp.Size == size, cancellationToken);
    }
}