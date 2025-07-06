using BrewBoxApi.Domain.Aggregates.Orders;
using BrewBoxApi.Infrastructure.Identity;
using BrewBoxApi.Presentation.Features.Orders.CreateOrderCommand;
using BrewBoxApi.Presentation.Features.Orders.Models;
using BrewBoxApi.Presentation.Features.Orders.UpdateOrderCommand;

namespace BrewBoxApi.Presentation.Features.Orders;

public sealed class OrderControllerImplementation(IOrderRepository orderRepository, ICurrentUserService currentUserService)
: IOrderControllerImplementation
{
    public async ValueTask<string> AddAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var order = new Order
        {
            Status = OrderStatus.Placed,
            PickupTime = request.PickupTime,
            TotalPrice = request.Drinks.Sum(d => d.Price),
            Tip = request.Tip,
            Paid = false,
            Drinks = [.. request.Drinks.Select(d => new Drink
            {
                Type = d.Type,
                Size = d.Size,
                Price = d.Price
            })]
        };

        var entity = await orderRepository.AddAsync(order, cancellationToken);
        return entity.Id;
    }

    public async ValueTask<List<OrderView>> GetAllActiveByUserIdAsync(CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User could not be found.");
        var list = await orderRepository.GetAllActiveByUserIdAsync(userId, cancellationToken);

        return [.. list.Select(OrderView.MapFrom)];
    }

    public async ValueTask<List<OrderView>> GetAllByUserIdAsync(CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User could not be found.");

        var list = await orderRepository.GetAllByUserIdAsync(userId, cancellationToken);

        return [.. list.Select(OrderView.MapFrom)];
    }

    public async ValueTask<OrderView> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        var entity = await orderRepository.GetByIdAsync(id, cancellationToken);
        return OrderView.MapFrom(entity);
    }

    public async ValueTask UpdateAsync(string id, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var order = await orderRepository.GetByIdAsync(id, cancellationToken);
        order.Status = request.Status;
        await orderRepository.UpdateAsync(order, cancellationToken);
    }
}
