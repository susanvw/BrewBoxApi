using BrewBoxApi.Domain.SeedWork;

namespace BrewBoxApi.Domain.Aggregates.Orders;

public class Drink : BaseModel
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public string Type { get; set; } = null!;
    public DrinkSize Size { get; set; }
    public decimal Price { get; set; }
}
