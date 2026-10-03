using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.ProblemReports.GetMyProblemReports;

public sealed record GetMyProblemReportsQuery : IRequest<Result<List<ProblemReportResponse>>>;
