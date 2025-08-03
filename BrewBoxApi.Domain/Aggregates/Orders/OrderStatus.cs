namespace BrewBoxApi.Domain.Aggregates.Orders;

public enum OrderStatus
{
    Placed,
    Claimed,
    InProgress,
    Ready,
    Collected,
    Cancelled,
    Paid
}