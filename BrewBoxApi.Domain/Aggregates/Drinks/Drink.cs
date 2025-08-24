using BrewBoxApi.Domain.SeedWork;

namespace BrewBoxApi.Domain.Aggregates.Drinks;

public class Drink : BaseModel
{
    public DrinkType Type { get; private set; }
    public DrinkSize Size { get; private set; }
    public decimal Price { get; private set; }

    // Private constructor for EF
    private Drink() { }

    public Drink(DrinkType type, DrinkSize size, decimal price)
    {
        if (price < 0) throw new ArgumentException("Price cannot be negative.");
        Type = type;
        Size = size;
        Price = price;
    }
}