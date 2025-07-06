using BrewBoxApi.Domain.Aggregates.Orders;
using BrewBoxApi.Infrastructure.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace BrewBoxApi.Infrastructure.Repositories;

public sealed class OrderRepository(ApplicationDbContext context) : BaseRepository<Order>(context), IOrderRepository
{
    public async ValueTask<Order> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        return await FindAsync(id, cancellationToken, o => o.Drinks, o => o.CreatedBy, o => o.Barista);
    }

    public async ValueTask<IEnumerable<Order>> GetAllByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        return await Where(o => o.CreatedById == userId, o => o.Drinks, o => o.CreatedBy, o => o.Barista)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<IEnumerable<Order>> GetAllActiveByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        return await Where(o => o.CreatedById == userId && o.Status != OrderStatus.Collected && o.Status != OrderStatus.Paid, o => o.Drinks, o => o.CreatedBy, o => o.Barista)
            .ToListAsync(cancellationToken);
    }
}