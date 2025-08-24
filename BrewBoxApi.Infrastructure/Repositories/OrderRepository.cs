using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using BrewBoxApi.Domain.Aggregates.Orders;
using BrewBoxApi.Domain.SeedWork;
using BrewBoxApi.Infrastructure.Data;

namespace BrewBoxApi.Infrastructure.Repositories;

public sealed class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async ValueTask<Order?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await FindAsync(id, cancellationToken, o => o.Drinks, o => o.CreatedBy, o => o.Barista);
    }

    public async ValueTask<IEnumerable<Order>> GetAllByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
    {
        return await Where(o => o.CreatedById == customerId, o => o.Drinks, o => o.CreatedBy, o => o.Barista)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<IEnumerable<Order>> GetOutstandingPaymentsByCustomerAsync(string customerId, CancellationToken cancellationToken = default)
    {
        return await Where(o => o.CreatedById == customerId && o.Status == OrderStatus.Collected && !o.Paid, o => o.Drinks, o => o.CreatedBy, o => o.Barista)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<IEnumerable<Order>> GetCurrentOrdersByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
    {
        return await Where(o => o.CreatedById == customerId && o.Status != OrderStatus.Collected, o => o.Drinks, o => o.CreatedBy, o => o.Barista)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<IEnumerable<Order>> GetCurrentOrdersByBaristaIdAsync(string baristaId, CancellationToken cancellationToken = default)
    {
        return await Where(o => (o.BaristaId == baristaId || o.BaristaId == null) && o.Status != OrderStatus.Collected, o => o.Drinks, o => o.CreatedBy, o => o.Barista)
            .ToListAsync(cancellationToken);
    }
}