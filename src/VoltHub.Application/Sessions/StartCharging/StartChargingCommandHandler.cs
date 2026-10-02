using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Entities;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Sessions.StartCharging;

// SSA process 4.1: charging starts only from the caller's active reservation, inside its time slot.
internal sealed class StartChargingCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<StartChargingCommand, Result<Guid>>
{
    private static readonly TimeSpan EarlyStart = TimeSpan.FromMinutes(5);   // arriving a little early is fine

    public async Task<Result<Guid>> Handle(StartChargingCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
            return CommonErrors.NotAuthenticated;

        var reservation = await db.Reservations
            .Include(r => r.Charger)
            .SingleOrDefaultAsync(r => r.Id == request.ReservationId && r.Vehicle.UserId == userId, cancellationToken);
        if (reservation is null)
            return SessionErrors.ReservationNotFound;

        var now = DateTime.UtcNow;
        if (reservation.Status != ReservationStatus.Active)
            return SessionErrors.ReservationNotActive;
        if (now < reservation.StartTime - EarlyStart || now >= reservation.EndTime)
            return SessionErrors.OutsideReservedSlot;
        if (reservation.Charger.Status != ChargerStatus.Available)
            return SessionErrors.ChargerUnavailable;
        if (await db.ChargingSessions.AnyAsync(s => s.ReservationId == reservation.Id, cancellationToken))
            return SessionErrors.AlreadyStarted;
        if (await db.ChargingSessions.AnyAsync(
                s => s.Reservation.ChargerId == reservation.ChargerId && s.Status == SessionStatus.InProgress, cancellationToken))
            return SessionErrors.ChargerOccupied;

        // The session freezes today's price, so a later price change can't alter this charge.
        var session = ChargingSession.Create(reservation.Charger.PricePerKwh, reservation.Id);
        db.ChargingSessions.Add(session);
        await db.SaveChangesAsync(cancellationToken);   // unique reservation_id blocks a double start
        return session.Id;
    }
}
