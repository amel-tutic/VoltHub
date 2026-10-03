using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.ProblemReports;

internal static class ProblemReportErrors
{
    public static readonly Error NotFound = Error.NotFound("ProblemReport.NotFound", "Problem report not found.");
    public static readonly Error ChargerNotFound = Error.NotFound("ProblemReport.ChargerNotFound", "Charger not found.");

    public static Error InvalidTransition(ProblemStatus from, ProblemStatus to) =>
        Error.Conflict("ProblemReport.InvalidTransition", $"A report that is {from} can't be changed to {to}.");
}
