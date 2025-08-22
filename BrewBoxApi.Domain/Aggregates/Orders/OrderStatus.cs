namespace BrewBoxApi.Domain.Aggregates.Orders;

public enum OrderStatus
{
    Placed,
    InProgress,
    Ready,
    Collected,
    Cancelled
}