using BrewBoxApi.Application.Common.Identity;
using BrewBoxApi.Application.CQRS.Orders.Models;
using BrewBoxApi.Domain.Aggregates.Orders;

namespace BrewBoxApi.Application.CQRS.Statements;

public sealed class StatementControllerImplementation(IOrderRepository orderRepository, ICurrentUserService currentUserService)
: IStatementControllerImplementation
{
    public ValueTask<List<OrderView>> GetOutstandingPaymentsByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
