using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Reports.GetOverviewReport;

internal sealed class GetOverviewReportQueryHandler(IApplicationDbContext db) : IRequestHandler<GetOverviewReportQuery, Result<OverviewReportResponse>>
{
    public async Task<Result<OverviewReportResponse>> Handle(GetOverviewReportQuery request, CancellationToken cancellationToken)
    {
        // A handful of COUNT queries; each one is cheap and easy to read.
        return new OverviewReportResponse(
            Owners: await db.Users.CountAsync(u => u.Role == UserRole.User, cancellationToken),
            ActiveOwners: await db.Users.CountAsync(u => u.Role == UserRole.User && u.IsActive, cancellationToken),
            Operators: await db.Users.CountAsync(u => u.Role == UserRole.Operator, cancellationToken),
            Admins: await db.Users.CountAsync(u => u.Role == UserRole.Admin, cancellationToken),
            Vehicles: await db.Vehicles.CountAsync(cancellationToken),
            Stations: await db.ChargingStations.CountAsync(cancellationToken),
            Chargers: await db.Chargers.CountAsync(cancellationToken),
            ChargersOutOfOrder: await db.Chargers.CountAsync(c => c.Status == ChargerStatus.OutOfOrder, cancellationToken),
            ActiveReservations: await db.Reservations.CountAsync(r => r.Status == ReservationStatus.Active, cancellationToken),
            CompletedSessions: await db.ChargingSessions.CountAsync(s => s.Status == SessionStatus.Completed, cancellationToken));
    }
}
