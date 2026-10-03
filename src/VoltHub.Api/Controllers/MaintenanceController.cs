using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoltHub.Api.Contracts;
using VoltHub.Application.Common.Security;
using VoltHub.Application.Maintenance.GetChargerMaintenance;
using VoltHub.Application.Maintenance.GetOpenMaintenance;
using VoltHub.Application.Maintenance.LogIntervention;
using VoltHub.Application.Maintenance.PlanMaintenance;
using VoltHub.Application.Maintenance.ReportFault;
using VoltHub.Application.Maintenance.ResolveMaintenance;

namespace VoltHub.Api.Controllers;

// SSA process 7: maintenance work on chargers, for operators and administrators.
[Route("api")]
[Authorize(Roles = Roles.Staff)]
public sealed class MaintenanceController(ISender sender) : ApiController
{
    [HttpGet("maintenance/open")]
    public async Task<IActionResult> Open(CancellationToken cancellationToken)
        => FromResult(await sender.Send(new GetOpenMaintenanceQuery(), cancellationToken));

    [HttpPost("maintenance/{id:guid}/resolve")]
    public async Task<IActionResult> Resolve(Guid id, CancellationToken cancellationToken)
        => FromResult(await sender.Send(new ResolveMaintenanceCommand(id), cancellationToken));

    [HttpGet("chargers/{id:guid}/maintenance")]
    public async Task<IActionResult> History(Guid id, CancellationToken cancellationToken)
        => FromResult(await sender.Send(new GetChargerMaintenanceQuery(id), cancellationToken));

    [HttpPost("chargers/{id:guid}/maintenance/faults")]
    public async Task<IActionResult> ReportFault(Guid id, FaultRequest request, CancellationToken cancellationToken)
        => FromCreated(await sender.Send(new ReportFaultCommand(id, request.Description), cancellationToken));

    [HttpPost("chargers/{id:guid}/maintenance/planned")]
    public async Task<IActionResult> Plan(Guid id, PlannedMaintenanceRequest request, CancellationToken cancellationToken)
        => FromCreated(await sender.Send(new PlanMaintenanceCommand(id, request.ScheduledDate, request.Description), cancellationToken));

    [HttpPost("chargers/{id:guid}/maintenance/interventions")]
    public async Task<IActionResult> LogIntervention(Guid id, InterventionRequest request, CancellationToken cancellationToken)
        => FromCreated(await sender.Send(new LogInterventionCommand(id, request.Type, request.Description), cancellationToken));
}
