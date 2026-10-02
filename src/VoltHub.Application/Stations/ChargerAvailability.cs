using VoltHub.Application.Common.Interfaces;
using VoltHub.Domain.Entities;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Stations;

// Only the base status (Available / OutOfOrder / UnderMaintenance) is stored.
// "Occupied" and "Reserved" are derived here from sessions and reservations, never saved.
internal static class ChargerAvailability
{
    // Chargers a driver could use right now: operational, not charging a car, not inside someone's reserved slot.
    public static IQueryable<Charger> AvailableNow(IApplicationDbContext db, DateTime now) =>
        db.Chargers.Where(c =>
            c.Status == ChargerStatus.Available &&
            !db.ChargingSessions.Any(s => s.Reservation.ChargerId == c.Id && s.Status == SessionStatus.InProgress) &&
            !db.Reservations.Any(r => r.ChargerId == c.Id && r.Status == ReservationStatus.Active &&
                                      r.StartTime <= now && r.EndTime > now));

    public static ChargerStatus Effective(ChargerStatus baseStatus, bool inSession, bool reservedNow) =>
        baseStatus != ChargerStatus.Available ? baseStatus
        : inSession ? ChargerStatus.Occupied
        : reservedNow ? ChargerStatus.Reserved
        : ChargerStatus.Available;
}
