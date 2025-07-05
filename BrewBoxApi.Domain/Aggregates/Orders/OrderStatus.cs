namespace BrewBoxApi.Domain.Aggregates.Orders;

public enum OrderStatus
{
    Placed,
    InProgress,
    Complete,
    Paid,
    Collected
}