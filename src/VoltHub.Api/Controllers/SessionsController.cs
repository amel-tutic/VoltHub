using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoltHub.Application.Common.Security;
using VoltHub.Application.Sessions.GetActiveSessions;
using VoltHub.Application.Sessions.GetMySessions;
using VoltHub.Application.Sessions.StartCharging;
using VoltHub.Application.Sessions.StopCharging;

namespace VoltHub.Api.Controllers;

[Route("api/sessions")]
[Authorize(Roles = Roles.User)]
public sealed class SessionsController(ISender sender) : ApiController
{
    [HttpPost("start")]
    public async Task<IActionResult> Start(StartChargingCommand command, CancellationToken cancellationToken)
        => FromCreated(await sender.Send(command, cancellationToken));

    [HttpGet("active")]
    public async Task<IActionResult> Active(CancellationToken cancellationToken)
        => FromResult(await sender.Send(new GetActiveSessionsQuery(), cancellationToken));

    [HttpPost("{id:guid}/stop")]
    public async Task<IActionResult> Stop(Guid id, CancellationToken cancellationToken)
        => FromResult(await sender.Send(new StopChargingCommand(id), cancellationToken));

    [HttpGet]
    public async Task<IActionResult> History(CancellationToken cancellationToken)
        => FromResult(await sender.Send(new GetMySessionsQuery(), cancellationToken));
}
