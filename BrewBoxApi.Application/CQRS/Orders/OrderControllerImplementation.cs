using FluentValidation;
using BrewBoxApi.Application.Common.Exceptions;
using BrewBoxApi.Application.Common.Identity;
using BrewBoxApi.Application.CQRS.Orders.CreateOrderCommand;
using BrewBoxApi.Application.CQRS.Orders.Models;
using BrewBoxApi.Application.CQRS.Orders.UpdateOrderCommand;
using BrewBoxApi.Domain.Aggregates.Drinks;
using BrewBoxApi.Domain.Aggregates.Orders;
using BrewBoxApi.Domain.Aggregates.Identity;

namespace BrewBoxApi.Application.CQRS.Orders;

public sealed class OrderControllerImplementation(
    IOrderRepository orderRepository,
    IDrinkRepository drinkRepository,
    ICurrentUserService currentUserService)
    : IOrderControllerImplementation
{
    const string AUTH_MESSAGE = "User not authenticated";
    public async ValueTask<OrderView> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        var order = await orderRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(id, nameof(Order));
        return OrderView.MapFrom(order);
    }

    public async ValueTask<List<OrderView>> GetAllByCustomerIdAsync(CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId ??
            throw new UnauthorizedAccessException(AUTH_MESSAGE);
        var orders = await orderRepository.GetAllByCustomerIdAsync(userId, cancellationToken);
        return [.. orders.Select(OrderView.MapFrom)];
    }

    public async ValueTask<List<OrderView>> GetCurrentOrdersByCustomerIdAsync(CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException(AUTH_MESSAGE);
        var orders = await orderRepository.GetCurrentOrdersByCustomerIdAsync(userId, cancellationToken);
        return [.. orders.Select(OrderView.MapFrom)];
    }

    public async ValueTask<List<OrderView>> GetCurrentOrdersByBaristaIdAsync(CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException(AUTH_MESSAGE);
        var orders = await orderRepository.GetCurrentOrdersByBaristaIdAsync(userId, cancellationToken);
        return [.. orders.Select(OrderView.MapFrom)];
    }

    public async ValueTask UpdateAsync(string id, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        if (request.OrderId != id) throw new ValidationException("OrderId in request must match route ID.");

        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException(AUTH_MESSAGE);

        var validator = new UpdateOrderStatusRequestValidator(orderRepository);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var order = await orderRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(id, nameof(Order));

        var newStatus = Enum.Parse<OrderStatus>(request.Status, true);

        var isBarista = await currentUserService.IsInRoleAsync(RoleType.Barista);
        // Restrict status changes based on role
        if (!isBarista && newStatus is OrderStatus.InProgress or OrderStatus.Ready)
            throw new UnauthorizedAccessException("Only baristas can set status to InProgress or Ready.");

        order.SetStatus(newStatus);
        if (newStatus == OrderStatus.InProgress && isBarista)
            order.BaristaId = userId;

        await orderRepository.UpdateAsync(order, cancellationToken);
    }

    public async ValueTask MarkAsPaidAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        var request = new MarkOrderAsPaidRequest(id);

        var validator = new MarkOrderAsPaidRequestValidator(orderRepository);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var order = await orderRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(id, nameof(Order));

        if (order.Paid)
            throw new ValidationException("Order is already marked as paid.");

        order.MarkAsPaid();
        await orderRepository.UpdateAsync(order, cancellationToken);
    }

    public async ValueTask<string> AddAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var validator = new CreateOrderRequestValidator(drinkRepository);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User not authenticated.");

        var drinks = new List<(Drink Drink, int Quantity, decimal Price)>();
        foreach (var drinkRequest in request.Drinks)
        {
            var drink = await drinkRepository.GetByTypeAndSizeAsync(drinkRequest.Type, drinkRequest.Size, cancellationToken)
                ?? throw new NotFoundException($" {drinkRequest.Type}-{drinkRequest.Size}", nameof(Drink));
            drinks.Add((drink, drinkRequest.Quantity, drink.Price));
        }

        var order = Order.Create(
            createdById: userId,
            pickupTime: request.PickupTime,
            tip: request.Tip,
            drinks: drinks
        );

        var entity = await orderRepository.AddAsync(order, cancellationToken);
        return entity.Id;
    }
}