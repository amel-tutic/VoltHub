using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoltHub.Application.Notifications.GetMyNotifications;
using VoltHub.Application.Notifications.MarkAllNotificationsRead;
using VoltHub.Application.Notifications.MarkNotificationRead;

namespace VoltHub.Api.Controllers;

[Route("api/notifications")]
[Authorize]
public sealed class NotificationsController(ISender sender) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetMine([FromQuery] bool unreadOnly, CancellationToken cancellationToken, [FromQuery] int take = 50)
        => FromResult(await sender.Send(new GetMyNotificationsQuery(unreadOnly, take), cancellationToken));

    [HttpPost("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken cancellationToken)
        => FromResult(await sender.Send(new MarkNotificationReadCommand(id), cancellationToken));

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllRead(CancellationToken cancellationToken)
        => FromResult(await sender.Send(new MarkAllNotificationsReadCommand(), cancellationToken));
}
