using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoltHub.Api.Contracts;
using VoltHub.Application.Common.Security;
using VoltHub.Application.ProblemReports.ChangeProblemReportStatus;
using VoltHub.Application.ProblemReports.CreateProblemReport;
using VoltHub.Application.ProblemReports.GetMyProblemReports;
using VoltHub.Application.ProblemReports.GetProblemReports;
using VoltHub.Domain.Enums;

namespace VoltHub.Api.Controllers;

[Route("api/problem-reports")]
[Authorize]
public sealed class ProblemReportsController(ISender sender) : ApiController
{
    [HttpPost]
    [Authorize(Roles = Roles.User)]
    public async Task<IActionResult> Create(CreateProblemReportCommand command, CancellationToken cancellationToken)
        => FromCreated(await sender.Send(command, cancellationToken));

    [HttpGet("mine")]
    [Authorize(Roles = Roles.User)]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
        => FromResult(await sender.Send(new GetMyProblemReportsQuery(), cancellationToken));

    [HttpGet]
    [Authorize(Roles = Roles.Staff)]
    public async Task<IActionResult> Get([FromQuery] ProblemStatus? status, CancellationToken cancellationToken)
        => FromResult(await sender.Send(new GetProblemReportsQuery(status), cancellationToken));

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = Roles.Staff)]
    public async Task<IActionResult> ChangeStatus(Guid id, ProblemStatusRequest request, CancellationToken cancellationToken)
        => FromResult(await sender.Send(new ChangeProblemReportStatusCommand(id, request.Status), cancellationToken));
}
