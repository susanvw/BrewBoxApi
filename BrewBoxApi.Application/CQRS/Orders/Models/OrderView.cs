using BrewBoxApi.Domain.Aggregates.Orders;

namespace BrewBoxApi.Application.CQRS.Orders.Models;

public sealed record OrderView
{
    public required string Id { get; set; }
    public required decimal TotalPrice { get; set; }
    public required string Status { get; set; }
    public DateTime PickupTime { get; set; }
    public string? Barista { get; set; }
    public required string Customer { get; set; }
    public decimal? Tip { get; set; }
    public bool Paid { get; set; }
    public required DrinkView[] Drinks { get; set; }

    internal static Func<Order, OrderView> MapFrom => (order) =>
    {
        return new OrderView
        {
            Id = order.Id,
            TotalPrice = order.Drinks.Select(x => x.Price).Sum(),
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