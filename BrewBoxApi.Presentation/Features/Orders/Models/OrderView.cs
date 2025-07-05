using BrewBoxApi.Domain.Aggregates.Orders;

namespace BrewBoxApi.Presentation.Features.Orders.Models;

public sealed record OrderView
{
    public required string Id { get; set; }

    internal static Func<Order, OrderView> MapFrom => (order) =>
    {
        return new OrderView
        {
            Id = order.Id
        };
    };
}