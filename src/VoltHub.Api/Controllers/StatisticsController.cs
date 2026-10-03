using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoltHub.Application.Common.Security;
using VoltHub.Application.Statistics.GetMyStatistics;

namespace VoltHub.Api.Controllers;

[Route("api/statistics")]
[Authorize(Roles = Roles.User)]
public sealed class StatisticsController(ISender sender) : ApiController
{
    [HttpGet("me")]
    public async Task<IActionResult> Mine(CancellationToken cancellationToken)
        => FromResult(await sender.Send(new GetMyStatisticsQuery(), cancellationToken));
}
