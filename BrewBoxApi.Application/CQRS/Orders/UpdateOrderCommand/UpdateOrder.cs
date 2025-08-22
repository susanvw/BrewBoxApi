using BrewBoxApi.Domain.Aggregates.Orders;

namespace BrewBoxApi.Application.CQRS.Orders.UpdateOrderCommand;

public record UpdateOrderStatusRequest(string Status);
