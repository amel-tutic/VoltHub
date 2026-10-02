using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Entities;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Reservations.CreateReservation;

internal sealed class CreateReservationCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<CreateReservationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
            return CommonErrors.NotAuthenticated;

        var start = request.StartTime.UtcDateTime;
        var end = request.EndTime.UtcDateTime;

        // 1. The vehicle must be one of the caller's own.
        var vehicle = await db.Vehicles.SingleOrDefaultAsync(v => v.Id == request.VehicleId && v.UserId == userId, cancellationToken);
        if (vehicle is null)
            return ReservationErrors.VehicleNotFound;

        // 2. The charger must exist, be operational, and fit the vehicle's plug.
        var charger = await db.Chargers.SingleOrDefaultAsync(c => c.Id == request.ChargerId, cancellationToken);
        if (charger is null)
            return ReservationErrors.ChargerNotFound;
        if (charger.Status != ChargerStatus.Available)
            return ReservationErrors.ChargerUnavailable(charger.Status);
        if (charger.ConnectorType != vehicle.ConnectorType)
            return ReservationErrors.ConnectorMismatch(vehicle.ConnectorType, charger.ConnectorType);

        // 3. No overlap. Slots are half-open [start, end): two overlap when each starts before the other ends.
        var overlapping = db.Reservations.Where(r =>
            r.Status == ReservationStatus.Active && r.StartTime < end && start < r.EndTime);

        if (await overlapping.AnyAsync(r => r.ChargerId == charger.Id, cancellationToken))
            return ReservationErrors.ChargerSlotTaken;
        if (await overlapping.AnyAsync(r => r.VehicleId == vehicle.Id, cancellationToken))
            return ReservationErrors.VehicleSlotTaken;

        var reservation = Reservation.Create(start, end, vehicle.Id, charger.Id);
        db.Reservations.Add(reservation);

        // If another request booked the same slot a millisecond ago, the database's exclusion
        // constraint rejects this insert and the API answers 409 — the checks above can't be raced past.
        await db.SaveChangesAsync(cancellationToken);
        return reservation.Id;
    }
}
