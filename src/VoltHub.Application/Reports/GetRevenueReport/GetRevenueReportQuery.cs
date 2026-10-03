using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Reports.GetRevenueReport;

public sealed record GetRevenueReportQuery(DateTimeOffset? From, DateTimeOffset? To)
    : IRequest<Result<RevenueReportResponse>>, IPeriodQuery;
