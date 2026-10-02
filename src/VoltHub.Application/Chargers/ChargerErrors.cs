using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Chargers;

internal static class ChargerErrors
{
    public static readonly Error NotFound = Error.NotFound("Charger.NotFound", "Charger not found.");

    public static Error CodeTaken(string code) =>
        Error.Conflict("Charger.CodeTaken", $"A charger with code '{code}' already exists at this station.");
}
