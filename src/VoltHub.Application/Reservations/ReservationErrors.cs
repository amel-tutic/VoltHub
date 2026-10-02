using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Reservations;

internal static class ReservationErrors
{
    public static readonly Error NotFound = Error.NotFound("Reservation.NotFound", "Reservation not found.");
    public static readonly Error VehicleNotFound = Error.NotFound("Reservation.VehicleNotFound", "Vehicle not found among your vehicles.");
    public static readonly Error ChargerNotFound = Error.NotFound("Reservation.ChargerNotFound", "Charger not found.");
    public static readonly Error ChargerSlotTaken = Error.Conflict("Reservation.ChargerSlotTaken",
        "The charger is already reserved for an overlapping time slot.");
    public static readonly Error VehicleSlotTaken = Error.Conflict("Reservation.VehicleSlotTaken",
        "This vehicle already has a reservation in an overlapping time slot.");
    public static readonly Error AlreadyStarted = Error.Conflict("Reservation.AlreadyStarted",
        "A reservation can only be cancelled before its time slot starts.");

    public static Error ChargerUnavailable(ChargerStatus status) =>
        Error.Conflict("Reservation.ChargerUnavailable", $"The charger can't be reserved while it is {status}.");

    public static Error ConnectorMismatch(ConnectorType vehicle, ConnectorType charger) =>
        Error.Validation("Reservation.ConnectorMismatch", $"The vehicle uses {vehicle}, but the charger has a {charger} connector.");

    public static Error NotActive(ReservationStatus status) =>
        Error.Conflict("Reservation.NotActive", $"The reservation is {status}, not Active.");
}
