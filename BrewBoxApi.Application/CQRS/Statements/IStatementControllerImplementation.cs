using BrewBoxApi.Application.CQRS.Orders.Models;

namespace BrewBoxApi.Application.CQRS.Statements;

public interface IStatementControllerImplementation
{
    ValueTask<List<OrderView>> GetOutstandingPaymentsByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);
}
