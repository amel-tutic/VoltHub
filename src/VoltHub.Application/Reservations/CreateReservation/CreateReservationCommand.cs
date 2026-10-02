using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Reservations.CreateReservation;

// DateTimeOffset carries the client's time zone ("2026-09-28T10:00:00+02:00"); the handler converts to UTC.
public sealed record CreateReservationCommand(Guid ChargerId, Guid VehicleId, DateTimeOffset StartTime, DateTimeOffset EndTime)
    : IRequest<Result<Guid>>;
