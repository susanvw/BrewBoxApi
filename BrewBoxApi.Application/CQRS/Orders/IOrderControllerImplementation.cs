using BrewBoxApi.Application.CQRS.Orders.CreateOrderCommand;
using BrewBoxApi.Application.CQRS.Orders.Models;
using BrewBoxApi.Application.CQRS.Orders.UpdateOrderCommand;

namespace BrewBoxApi.Application.CQRS.Orders;

public interface IOrderControllerImplementation
{
    ValueTask<OrderView> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    ValueTask<List<OrderView>> GetAllByCustomerIdAsync(CancellationToken cancellationToken = default);
    ValueTask<List<OrderView>> GetCurrentOrdersByCustomerIdAsync(CancellationToken cancellationToken = default);
    ValueTask<List<OrderView>> GetCurrentOrdersByBaristaIdAsync(CancellationToken cancellationToken = default);
    ValueTask UpdateAsync(string id, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default);
    ValueTask MarkAsPaidAsync(string id, CancellationToken cancellationToken = default);
    ValueTask<string> AddAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
}
