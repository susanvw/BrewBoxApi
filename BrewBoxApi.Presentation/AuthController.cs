using System.Security.Claims;
using BrewBoxApi.Application.CQRS.Auth;
using BrewBoxApi.Application.CQRS.Auth.LoginCommand;
using Microsoft.AspNetCore.Mvc;

namespace BrewBoxApi.Presentation;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthControllerImplementation implementation) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult> LoginAsync([FromBody] LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await implementation.LoginAsync(request, cancellationToken);
        if (!result.Success)
        {
            return Unauthorized(new { errors = result.Errors });
        }

        return Ok(result);
    }
}