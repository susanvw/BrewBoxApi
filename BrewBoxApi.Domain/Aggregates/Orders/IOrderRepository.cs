using BrewBoxApi.Domain.SeedWork;

namespace BrewBoxApi.Domain.Aggregates.Orders;

public interface IOrderRepository : IBaseReadRepository<Order>, IBaseCommandRepository<Order>
{
    /// <summary>
    /// Get all orders for a user (past and present).
    /// </summary>
    ValueTask<IEnumerable<Order>> GetAllByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all orders that have not been paid yet.
    /// </summary>
    ValueTask<IEnumerable<Order>> GetOutstandingPaymentsByCustomerAsync(string customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all current orders for a user - not yet collected.
    /// </summary>
    ValueTask<IEnumerable<Order>> GetCurrentOrdersByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the current orders for a barista.
    /// </summary>
    ValueTask<IEnumerable<Order>> GetCurrentOrdersByBaristaIdAsync(string baristaId, CancellationToken cancellationToken = default);
}