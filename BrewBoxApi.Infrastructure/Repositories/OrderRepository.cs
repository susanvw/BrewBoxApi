using BrewBoxApi.Domain.Aggregates.Orders;
using BrewBoxApi.Infrastructure.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace BrewBoxApi.Infrastructure.Repositories;

public sealed class OrderRepository(ApplicationDbContext context) : BaseRepository<Order>(context), IOrderRepository
{
    public async ValueTask<IEnumerable<Order>> GetAllByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);
        return await Where(o => o.CreatedById == customerId, o => o.Drinks)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<IEnumerable<Order>> GetCurrentOrdersByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);
        return await Where(o => o.CreatedById == customerId && o.Status != OrderStatus.Collected && !o.Paid, o => o.Drinks)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<IEnumerable<Order>> GetCurrentOrdersByBaristaIdAsync(string baristaId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baristaId);
        return await Where(o => (o.BaristaId == baristaId) && o.Status != OrderStatus.Collected && !o.Paid, o => o.Drinks)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<IEnumerable<Order>> GetOutstandingPaymentsByCustomerAsync(string customerId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);
        return await Where(o => o.CreatedById == customerId && o.Status == OrderStatus.Collected && !o.Paid, o => o.Drinks)
            .ToListAsync(cancellationToken);
    }
}