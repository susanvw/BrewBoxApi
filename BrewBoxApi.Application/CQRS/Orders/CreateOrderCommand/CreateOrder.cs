using BrewBoxApi.Domain.Aggregates.Orders;

namespace BrewBoxApi.Application.CQRS.Orders.CreateOrderCommand;

public record CreateDrinkRequest(string Type, string Size, decimal Price);
public record CreateOrderRequest(DateTime PickupTime, decimal? Tip, List<CreateDrinkRequest> Drinks);
