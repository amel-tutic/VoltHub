using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Reports.GetFaultReport;

public sealed record GetFaultReportQuery(DateTimeOffset? From, DateTimeOffset? To)
    : IRequest<Result<List<FaultReportResponse>>>, IPeriodQuery;
