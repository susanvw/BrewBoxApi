using BrewBoxApi.Application.CQRS.Statements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrewBoxApi.Presentation.Controllers.v1;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StatementController(IStatementControllerImplementation implementation) : ControllerBase
{

    [HttpGet("{customerId}")]
    [Authorize(Roles = "Barista, Customer")]
    public async Task<IActionResult> GetOutstandingPaymentsByCustomerIdAsync([FromRoute] string customerId, CancellationToken cancellationToken = default)
    {
        await implementation.GetOutstandingPaymentsByCustomerIdAsync(customerId, cancellationToken);
        return NoContent();
    }
}