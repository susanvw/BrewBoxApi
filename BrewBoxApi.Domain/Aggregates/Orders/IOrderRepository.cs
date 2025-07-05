namespace BrewBoxApi.Domain.Aggregates.Orders;

public interface IOrderRepository
{
    ValueTask<Order> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    ValueTask<IEnumerable<Order>> GetAllByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    ValueTask<IEnumerable<Order>> GetAllActiveByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    ValueTask<Order> AddAsync(Order entity, CancellationToken cancellationToken = default);
    ValueTask UpdateAsync(Order entity, CancellationToken cancellationToken = default);
    ValueTask DeleteAsync(string id, CancellationToken cancellationToken = default);
}
