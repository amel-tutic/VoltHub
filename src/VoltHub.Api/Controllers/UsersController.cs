using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoltHub.Api.Contracts;
using VoltHub.Application.Common.Security;
using VoltHub.Application.Users.CreateOperator;
using VoltHub.Application.Users.GetUsers;
using VoltHub.Application.Users.SetUserActive;
using VoltHub.Domain.Enums;

namespace VoltHub.Api.Controllers;

[Route("api/users")]
[Authorize(Roles = Roles.Admin)]
public sealed class UsersController(ISender sender) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] UserRole? role, [FromQuery] bool? isActive, [FromQuery] string? search, CancellationToken cancellationToken)
        => FromResult(await sender.Send(new GetUsersQuery(role, isActive, search), cancellationToken));

    [HttpPost("operators")]
    public async Task<IActionResult> CreateOperator(CreateOperatorCommand command, CancellationToken cancellationToken)
        => FromCreated(await sender.Send(command, cancellationToken));

    [HttpPatch("{id:guid}/active")]
    public async Task<IActionResult> SetActive(Guid id, UserActiveRequest request, CancellationToken cancellationToken)
        => FromResult(await sender.Send(new SetUserActiveCommand(id, request.IsActive), cancellationToken));
}
