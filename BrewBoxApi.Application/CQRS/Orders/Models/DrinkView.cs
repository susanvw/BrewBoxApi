using BrewBoxApi.Domain.Aggregates.Orders;

namespace BrewBoxApi.Application.CQRS.Orders.Models;

public sealed record DrinkView
{
    public required string Id { get; set; }
    public required string OrderId { get; set; }
    public required string Type { get; set; }
    public required string Size { get; set; }
    public decimal Price { get; set; }
    internal static Func<Drink, DrinkView> MapFrom => (drink) =>
    {
        return new DrinkView
        {
            Id = drink.Id,
            OrderId = drink.OrderId,
            Size = drink.Size.ToString(),
            Type = drink.Type.ToString(),
            Price = drink.Price,
        };
    };
}