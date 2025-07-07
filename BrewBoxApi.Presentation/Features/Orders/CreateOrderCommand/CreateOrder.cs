using BrewBoxApi.Domain.Aggregates.Orders;

namespace BrewBoxApi.Presentation.Features.Orders.CreateOrderCommand;

public record CreateDrinkRequest(string Type, string Size, decimal Price);
public record CreateOrderRequest(DateTime PickupTime, decimal? Tip, List<CreateDrinkRequest> Drinks);
