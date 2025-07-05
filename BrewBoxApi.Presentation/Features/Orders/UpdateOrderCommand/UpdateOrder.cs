using BrewBoxApi.Domain.Aggregates.Orders;

namespace BrewBoxApi.Presentation.Features.Orders.UpdateOrderCommand;

public record UpdateOrderStatusRequest(OrderStatus Status);
