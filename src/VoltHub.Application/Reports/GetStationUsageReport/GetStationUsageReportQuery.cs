using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Reports.GetStationUsageReport;

public sealed record GetStationUsageReportQuery(DateTimeOffset? From, DateTimeOffset? To)
    : IRequest<Result<List<StationUsageReportResponse>>>, IPeriodQuery;
