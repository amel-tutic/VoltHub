using MediatR;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.ProblemReports.ChangeProblemReportStatus;

// Allowed: Open -> InProgress, Open -> Resolved, InProgress -> Resolved.
public sealed record ChangeProblemReportStatusCommand(Guid Id, ProblemStatus Status) : IRequest<Result>;
