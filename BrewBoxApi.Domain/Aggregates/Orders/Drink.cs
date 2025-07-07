using BrewBoxApi.Domain.SeedWork;

namespace BrewBoxApi.Domain.Aggregates.Orders;

public class Drink : BaseModel
{
    public string OrderId { get; set; } = string.Empty;
    public Order Order { get; set; } = null!;
    public DrinkType Type { get; set; }
    public DrinkSize Size { get; set; }
    public decimal Price { get; set; }
}
