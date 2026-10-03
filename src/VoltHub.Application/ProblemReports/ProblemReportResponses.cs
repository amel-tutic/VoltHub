using VoltHub.Domain.Enums;

namespace VoltHub.Application.ProblemReports;

public sealed record ProblemReportResponse(
    Guid Id, ProblemType Type, string Description, ProblemStatus Status, DateTime CreatedAt, DateTime? ResolvedAt,
    Guid ChargerId, string ChargerCode, Guid StationId, string StationName, string ReporterName);
