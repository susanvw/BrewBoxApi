using BrewBoxApi.Domain.Aggregates.Drinks;
using BrewBoxApi.Domain.Aggregates.Identity;
using BrewBoxApi.Domain.SeedWork;

namespace BrewBoxApi.Domain.Aggregates.Orders;

public class Order : BaseModel
{
    public string? BaristaId { get; set; }
    public ApplicationUser? Barista { get; set; }
    public OrderStatus Status { get; private set; }
    public DateTime PickupTime { get; private set; }
    public decimal TotalPrice { get; private set; }
    public decimal? Tip { get; set; }
    public bool Paid { get; private set; }
    public List<OrderDrink> Drinks { get; private set; } = [];

    private Order() { } // For EF

    public static Order Create(string createdById, DateTime pickupTime, decimal? tip, List<(Drink Drink, int Quantity, decimal Price)> drinks)
    {
        if (pickupTime <= DateTime.UtcNow) throw new ArgumentException("Pickup time must be in the future.");
        if (string.IsNullOrWhiteSpace(createdById)) throw new ArgumentException("CreatedById must not be empty.");
        if (tip < 0) throw new ArgumentException("Tip cannot be negative.");
        if (!drinks.Any()) throw new ArgumentException("Order must contain at least one drink.");

        var order = new Order
        {
            CreatedById = createdById,
            Status = OrderStatus.Placed,
            PickupTime = pickupTime,
            Tip = tip,
            Paid = false
        };
        foreach (var (drink, quantity, price) in drinks)
        {
            order.Drinks.Add(new OrderDrink(order.Id, drink.Id, price, quantity));
        }
        order.TotalPrice = order.Drinks.Sum(d => d.Price * d.Quantity);
        return order;
    }

    public void SetStatus(OrderStatus status)
    {
        Status = status;
    }

    public void MarkAsPaid()
    {
        Paid = true;
    }
}