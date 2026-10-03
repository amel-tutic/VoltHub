using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Reports.GetOverviewReport;

public sealed record GetOverviewReportQuery : IRequest<Result<OverviewReportResponse>>;
