using BrewBoxApi.Domain.SeedWork;
using Microsoft.AspNetCore.Identity;

namespace BrewBoxApi.Domain.Aggregates.Orders
{
    public class Order : BaseModel
    {
        public string? BaristaId { get; set; }
        public IdentityUser? Barista { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime PickupTime { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal? Tip { get; set; }
        public bool Paid { get; set; }
        public List<Drink> Drinks { get; set; } = [];
    }
}