using BrewBoxApi.Application.CQRS.Orders.CreateOrderCommand;
using BrewBoxApi.Application.CQRS.Orders.Models;
using BrewBoxApi.Application.CQRS.Orders.UpdateOrderCommand;

namespace BrewBoxApi.Application.CQRS.Orders;

public interface IOrderControllerImplementation
{
    ValueTask<string> AddAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
    ValueTask<List<OrderView>> GetAllActiveByUserIdAsync(CancellationToken cancellationToken = default);
    ValueTask<List<OrderView>> GetAllByUserIdAsync(CancellationToken cancellationToken = default);
    ValueTask<OrderView> GetByIdAsync(string id, CancellationToken cancellationToken= default);
    ValueTask<List<OrderView>>  GetCurrentOrdersAsync(CancellationToken cancellationToken = default);
    ValueTask UpdateAsync(string id, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default);
}
