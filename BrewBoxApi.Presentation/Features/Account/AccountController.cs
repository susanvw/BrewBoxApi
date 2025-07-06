using BrewBoxApi.Presentation.Features.Account.MfaCommand;
using BrewBoxApi.Presentation.Features.Account.RegisterCommand;
using Microsoft.AspNetCore.Mvc;

namespace BrewBoxApi.Presentation.Features.Account;

[ApiController]
[Route("api/[controller]")]
public class AccountController(IAccountControllerImplementation implementation) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult> RegisterAsync([FromBody] RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var result = await implementation.RegisterAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest(new { error = "Registration failed", details = result.Errors.Select(e => e.Description) });
        }

        return Ok(new { message = "User registered successfully" });
    }

    [HttpPost("mfa/google")]
    public async Task<IActionResult> VerifyGoogleMfa([FromBody] MfaRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await implementation.VerifyGoogleMfaAsync(request, cancellationToken);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Invalid Google token: " + ex.Message });
        }
    }

    [HttpPost("mfa/apple")]
    public async Task<IActionResult> VerifyAppleMfaAsync([FromBody] MfaRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await implementation.VerifyAppleMfaAsync(request, cancellationToken);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Invalid Apple token: " + ex.Message });
        }
    }
}