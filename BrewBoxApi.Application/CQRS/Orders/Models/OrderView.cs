using BrewBoxApi.Domain.Aggregates.Orders;

namespace BrewBoxApi.Application.CQRS.Orders.Models;

public sealed record OrderView
{
    /// <summary>
    /// Unique identifier for the order.
    /// </summary>
    public required string Id { get; init; }
    /// <summary>
    /// Total price of the order (sum of drink prices * quantities).
    /// </summary>
    public required decimal TotalPrice { get; init; }
    /// <summary>
    /// Status of the order (e.g., Placed, InProgress).
    /// </summary>
    public required string Status { get; init; }
    /// <summary>
    /// Scheduled pickup time for the order.
    /// </summary>
    public DateTime PickupTime { get; init; }
    /// <summary>
    /// Display name of the assigned barista, if any.
    /// </summary>
    public string? Barista { get; init; }
    /// <summary>
    /// Display name of the customer who placed the order.
    /// </summary>
    public required string Customer { get; init; }
    /// <summary>
    /// Optional tip amount for the order.
    /// </summary>
    public decimal? Tip { get; init; }
    /// <summary>
    /// Whether the order has been paid.
    /// </summary>
    public bool Paid { get; init; }
    /// <summary>
    /// Drinks included in the order.
    /// </summary>
    public required DrinkView[] Drinks { get; init; }

    internal static Func<Order, OrderView> MapFrom => (order) =>
    {
        return new OrderView
        {
            Id = order.Id,
            TotalPrice = order.TotalPrice,
            Status = order.Status.ToString(),
            PickupTime = order.PickupTime,
            Barista = order.Barista?.DisplayName,
            Customer = order.CreatedBy.DisplayName,
            Tip = order.Tip,
            Paid = order.Paid,
            Drinks = [.. order.Drinks.Select(DrinkView.MapFrom)]
        };
    };
}