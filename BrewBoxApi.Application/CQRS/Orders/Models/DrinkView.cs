using BrewBoxApi.Domain.Aggregates.Orders;

namespace BrewBoxApi.Application.CQRS.Orders.Models;

public sealed record DrinkView
{
    /// <summary>
    /// Unique identifier for the drink in the order (from OrderDrink).
    /// </summary>
    public required string Id { get; init; } // Confirm Id exists in OrderDrink
    /// <summary>
    /// ID of the order this drink belongs to.
    /// </summary>
    public required string OrderId { get; init; }
    /// <summary>
    /// Type of the drink (e.g., Espresso, Latte).
    /// </summary>
    public required string DrinkType { get; init; }
    /// <summary>
    /// Size of the drink (e.g., Small, Medium, Large).
    /// </summary>
    public required string DrinkSize { get; init; }
    /// <summary>
    /// Number of this drink in the order.
    /// </summary>
    public required int Quantity { get; init; }
    /// <summary>
    /// Price per unit of the drink (snapshotted).
    /// </summary>
    public decimal Price { get; init; }

    internal static Func<OrderDrink, DrinkView> MapFrom => (drink) =>
    {
        return new DrinkView
        {
            Id = drink.Id, // Ensure Id exists in OrderDrink
            OrderId = drink.OrderId,
            DrinkType = drink.Drink.Type.ToString(),
            DrinkSize = drink.Drink.Size.ToString(),
            Price = drink.Price,
            Quantity = drink.Quantity
        };
    };
}