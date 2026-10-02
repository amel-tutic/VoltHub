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
        => FromResult(await sender.Send(command, cancellationToken));

    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Login(LoginCommand command, CancellationToken cancellationToken)
        => FromResult(await sender.Send(command, cancellationToken));

    [HttpPost("refresh")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Refresh(RefreshTokenCommand command, CancellationToken cancellationToken)
        => FromResult(await sender.Send(command, cancellationToken));

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
        => FromResult(await sender.Send(new GetMeQuery(), cancellationToken));
}
