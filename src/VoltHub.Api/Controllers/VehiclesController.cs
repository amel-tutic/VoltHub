using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoltHub.Api.Contracts;
using VoltHub.Application.Common.Security;
using VoltHub.Application.Vehicles.AddVehicle;
using VoltHub.Application.Vehicles.DeleteVehicle;
using VoltHub.Application.Vehicles.GetMyVehicles;
using VoltHub.Application.Vehicles.UpdateVehicle;

namespace VoltHub.Api.Controllers;

[Route("api/vehicles")]
[Authorize(Roles = Roles.User)]   // EV owners only
public sealed class VehiclesController(ISender sender) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
        => FromResult(await sender.Send(new GetMyVehiclesQuery(), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Add(AddVehicleCommand command, CancellationToken cancellationToken)
        => FromCreated(await sender.Send(command, cancellationToken));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, VehicleRequest request, CancellationToken cancellationToken)
        => FromResult(await sender.Send(new UpdateVehicleCommand(
            id, request.Make, request.Model, request.BatteryCapacityKwh, request.ConnectorType), cancellationToken));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => FromResult(await sender.Send(new DeleteVehicleCommand(id), cancellationToken));
}
