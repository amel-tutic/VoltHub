using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoltHub.Api.Contracts;
using VoltHub.Application.Chargers.SetChargerPrice;
using VoltHub.Application.Chargers.SetChargerStatus;
using VoltHub.Application.Common.Security;

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
}
