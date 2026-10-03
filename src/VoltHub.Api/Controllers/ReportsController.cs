using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoltHub.Application.Common.Security;
using VoltHub.Application.Reports.GetFaultReport;
using VoltHub.Application.Reports.GetOverviewReport;
using VoltHub.Application.Reports.GetRevenueReport;
using VoltHub.Application.Reports.GetStationUsageReport;

namespace VoltHub.Api.Controllers;

// SSA process 9: read-only reports for the administrator. Periods default to the last 30 days.
[Route("api/reports")]
[Authorize(Roles = Roles.Admin)]
public sealed class ReportsController(ISender sender) : ApiController
{
    [HttpGet("overview")]
    public async Task<IActionResult> Overview(CancellationToken cancellationToken)
        => FromResult(await sender.Send(new GetOverviewReportQuery(), cancellationToken));

    [HttpGet("stations")]
    public async Task<IActionResult> Stations([FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to, CancellationToken cancellationToken)
        => FromResult(await sender.Send(new GetStationUsageReportQuery(from, to), cancellationToken));

    [HttpGet("revenue")]
    public async Task<IActionResult> Revenue([FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to, CancellationToken cancellationToken)
        => FromResult(await sender.Send(new GetRevenueReportQuery(from, to), cancellationToken));

    [HttpGet("faults")]
    public async Task<IActionResult> Faults([FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to, CancellationToken cancellationToken)
        => FromResult(await sender.Send(new GetFaultReportQuery(from, to), cancellationToken));
}
