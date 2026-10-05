using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoltHub.Api.Contracts;
using VoltHub.Application.Chargers.DeleteCharger;
using VoltHub.Application.Chargers.SetChargerPrice;
using VoltHub.Application.Chargers.SetChargerStatus;
using VoltHub.Application.Common.Security;
using VoltHub.Application.Reservations.GetChargerSchedule;

namespace VoltHub.Api.Controllers;

[Route("api/chargers")]
[Authorize]
public sealed class ChargersController(ISender sender) : ApiController
{
    [HttpPatch("{id:guid}/price")]
    [Authorize(Roles = Roles.Staff)]
    public async Task<IActionResult> SetPrice(Guid id, ChargerPriceRequest request, CancellationToken cancellationToken)
        => FromResult(await sender.Send(new SetChargerPriceCommand(id, request.PricePerKwh), cancellationToken));

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = Roles.Staff)]
    public async Task<IActionResult> SetStatus(Guid id, ChargerStatusRequest request, CancellationToken cancellationToken)
        => FromResult(await sender.Send(new SetChargerStatusCommand(id, request.Status), cancellationToken));

    // Only a charger that was never used; otherwise 409 with a message to set it out of order.
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Staff)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => FromResult(await sender.Send(new DeleteChargerCommand(id), cancellationToken));

    [HttpGet("{id:guid}/schedule")]
    public async Task<IActionResult> Schedule(
        Guid id, [FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to, CancellationToken cancellationToken)
        => FromResult(await sender.Send(new GetChargerScheduleQuery(id, from, to), cancellationToken));
}
