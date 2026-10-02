using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoltHub.Api.Contracts;
using VoltHub.Application.Chargers.AddCharger;
using VoltHub.Application.Common.Security;
using VoltHub.Application.Stations.CreateStation;
using VoltHub.Application.Stations.DeleteStation;
using VoltHub.Application.Stations.GetNearestStations;
using VoltHub.Application.Stations.GetStationById;
using VoltHub.Application.Stations.GetStations;
using VoltHub.Application.Stations.UpdateStation;
using VoltHub.Domain.Enums;

namespace VoltHub.Api.Controllers;

[Route("api/stations")]
[Authorize]                                   // browsing is for logged-in users (requirements, module 2)
public sealed class StationsController(ISender sender) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string? city, [FromQuery] ConnectorType? connectorType, [FromQuery] CurrentType? currentType,
        [FromQuery] decimal? minPowerKw, [FromQuery] bool onlyAvailable, CancellationToken cancellationToken)
        => FromResult(await sender.Send(
            new GetStationsQuery(city, connectorType, currentType, minPowerKw, onlyAvailable), cancellationToken));

    [HttpGet("nearest")]
    public async Task<IActionResult> Nearest(
        [FromQuery] double latitude, [FromQuery] double longitude, [FromQuery] ConnectorType? connectorType,
        CancellationToken cancellationToken, [FromQuery] int take = 5)
        => FromResult(await sender.Send(
            new GetNearestStationsQuery(latitude, longitude, connectorType, take), cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => FromResult(await sender.Send(new GetStationByIdQuery(id), cancellationToken));

    [HttpPost]
    [Authorize(Roles = Roles.Staff)]
    public async Task<IActionResult> Create(CreateStationCommand command, CancellationToken cancellationToken)
        => FromCreated(await sender.Send(command, cancellationToken));

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Staff)]
    public async Task<IActionResult> Update(Guid id, StationRequest request, CancellationToken cancellationToken)
        => FromResult(await sender.Send(new UpdateStationCommand(
            id, request.Name, request.Address, request.City, request.Latitude, request.Longitude, request.Description), cancellationToken));

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Staff)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => FromResult(await sender.Send(new DeleteStationCommand(id), cancellationToken));

    [HttpPost("{id:guid}/chargers")]
    [Authorize(Roles = Roles.Staff)]
    public async Task<IActionResult> AddCharger(Guid id, ChargerRequest request, CancellationToken cancellationToken)
        => FromCreated(await sender.Send(new AddChargerCommand(
            id, request.Code, request.ConnectorType, request.CurrentType, request.PowerKw, request.PricePerKwh), cancellationToken));
}
