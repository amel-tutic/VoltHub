using MediatR;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.ProblemReports.GetProblemReports;

// SSA process 8.5: the operator's view of every report, optionally by status.
public sealed record GetProblemReportsQuery(ProblemStatus? Status) : IRequest<Result<List<ProblemReportResponse>>>;
