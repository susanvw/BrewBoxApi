using BrewBoxApi.Application.CQRS.Account;
using BrewBoxApi.Application.CQRS.Account.MfaCommand;
using BrewBoxApi.Application.CQRS.Account.RegisterCommand;
using Microsoft.AspNetCore.Mvc;

namespace BrewBoxApi.Presentation.Controllers.v1;

[ApiController]
[Route("api/[controller]")]
public class AccountController(IAccountControllerImplementation implementation) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult> RegisterAsync([FromBody] RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var result = await implementation.RegisterAsync(request, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new { error = "Registration failed", details = result.Errors });
        }

        return Ok(result);
    }
}