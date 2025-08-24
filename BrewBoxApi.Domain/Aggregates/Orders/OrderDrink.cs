using BrewBoxApi.Domain.Aggregates.Drinks;
using BrewBoxApi.Domain.SeedWork;

namespace BrewBoxApi.Domain.Aggregates.Orders;

public class OrderDrink : BaseModel
{
    public string OrderId { get; set; } = null!;
    public Order Order { get; set; } = null!;
    public string DrinkId { get; set; } = null!;
    public Drink Drink { get; set; } = null!;
    public decimal Price { get; private set; } // Snapshotted
    public int Quantity { get; private set; } = 1; // Optional

    private OrderDrink() { } // For EF

    public OrderDrink(string orderId, string drinkId, decimal price, int quantity = 1)
    {
        OrderId = orderId;
        DrinkId = drinkId;
        Price = price;
        Quantity = quantity > 0 ? quantity : throw new ArgumentException("Quantity must be positive.");
    }
}