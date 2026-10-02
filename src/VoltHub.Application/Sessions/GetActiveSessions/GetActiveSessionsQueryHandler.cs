using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Sessions.GetActiveSessions;

internal sealed class GetActiveSessionsQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetActiveSessionsQuery, Result<List<ActiveSessionResponse>>>
{
    public async Task<Result<List<ActiveSessionResponse>>> Handle(GetActiveSessionsQuery request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
            return CommonErrors.NotAuthenticated;

        var sessions = await db.ChargingSessions
            .Where(s => s.Status == SessionStatus.InProgress && s.Reservation.Vehicle.UserId == userId)
            .Include(s => s.Reservation).ThenInclude(r => r.Charger).ThenInclude(c => c.Station)
            .Include(s => s.Reservation).ThenInclude(r => r.Vehicle)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;
        return sessions.Select(s =>
        {
            var charger = s.Reservation.Charger;
            var vehicle = s.Reservation.Vehicle;
            // Same formula the final bill will use (ChargingSession.CalculateEnergyKwh), just evaluated "so far".
            var energy = s.EstimateEnergyKwh(now, charger.PowerKw, vehicle.BatteryCapacityKwh);
            return new ActiveSessionResponse(
                s.Id, s.ReservationId, charger.Station.Name, charger.Code, $"{vehicle.Make} {vehicle.Model}",
                s.StartedAt, Math.Round((now - s.StartedAt).TotalMinutes, 1),
                energy, s.PricePerKwh, Math.Round(energy * s.PricePerKwh, 2), s.Reservation.EndTime);
        }).ToList();
    }
}
