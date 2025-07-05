
using BrewBoxApi.Presentation.Features.Orders.CreateOrderCommand;
using BrewBoxApi.Presentation.Features.Orders.Models;
using BrewBoxApi.Presentation.Features.Orders.UpdateOrderCommand;

namespace BrewBoxApi.Presentation.Features.Orders;

public interface IOrderControllerImplementation
{
    ValueTask<string> AddAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
    ValueTask<List<OrderView>> GetAllActiveByUserIdAsync(CancellationToken cancellationToken = default);
    ValueTask<List<OrderView>> GetAllByUserIdAsync(CancellationToken cancellationToken = default);
    ValueTask<OrderView> GetByIdAsync(string id, CancellationToken cancellationToken= default);
    ValueTask UpdateAsync(string id, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default);
}
