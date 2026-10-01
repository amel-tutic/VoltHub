using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using VoltHub.Application.Auth.GetMe;
using VoltHub.Application.Auth.Login;
using VoltHub.Application.Auth.Refresh;
using VoltHub.Application.Auth.Register;

namespace VoltHub.Api.Controllers;

[Route("api/auth")]
public sealed class AuthController(ISender sender) : ApiController
{
    [HttpPost("register")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Register(RegisterCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : Problem(result.Error!);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Login(LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : Problem(result.Error!);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Refresh(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : Problem(result.Error!);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMeQuery(), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : Problem(result.Error!);
    }
}
