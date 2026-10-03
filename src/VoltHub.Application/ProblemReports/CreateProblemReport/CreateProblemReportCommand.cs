using MediatR;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.ProblemReports.CreateProblemReport;

// SSA process 8.4: a driver reports a problem with a specific charger.
public sealed record CreateProblemReportCommand(Guid ChargerId, ProblemType Type, string Description) : IRequest<Result<Guid>>;
