using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoltHub.Application.Common.Security;
using VoltHub.Application.Reservations.CancelReservation;
using VoltHub.Application.Reservations.CreateReservation;
using VoltHub.Application.Reservations.GetMyReservations;
using VoltHub.Domain.Enums;

namespace VoltHub.Api.Controllers;

[Route("api/reservations")]
[Authorize(Roles = Roles.User)]
public sealed class ReservationsController(ISender sender) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetMine([FromQuery] ReservationStatus? status, CancellationToken cancellationToken)
        => FromResult(await sender.Send(new GetMyReservationsQuery(status), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(CreateReservationCommand command, CancellationToken cancellationToken)
        => FromCreated(await sender.Send(command, cancellationToken));

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
        => FromResult(await sender.Send(new CancelReservationCommand(id), cancellationToken));
}
