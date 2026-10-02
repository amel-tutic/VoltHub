using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Sessions;

internal static class SessionErrors
{
    public static readonly Error NotFound = Error.NotFound("Session.NotFound", "Charging session not found.");
    public static readonly Error ReservationNotFound = Error.NotFound("Session.ReservationNotFound", "Reservation not found.");
    public static readonly Error ReservationNotActive = Error.Conflict("Session.ReservationNotActive",
        "Charging can only start from an active reservation.");
    public static readonly Error OutsideReservedSlot = Error.Conflict("Session.OutsideReservedSlot",
        "Charging can only start within the reserved time slot.");
    public static readonly Error ChargerUnavailable = Error.Conflict("Session.ChargerUnavailable",
        "The charger is out of order or under maintenance.");
    public static readonly Error AlreadyStarted = Error.Conflict("Session.AlreadyStarted",
        "Charging has already been started for this reservation.");
    public static readonly Error ChargerOccupied = Error.Conflict("Session.ChargerOccupied",
        "Another vehicle is still charging at this charger.");
    public static readonly Error NotInProgress = Error.Conflict("Session.NotInProgress", "This charging session has already ended.");
}
