using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoltHub.Application.Account.ChangePassword;
using VoltHub.Application.Account.UpdateProfile;

namespace VoltHub.Api.Controllers;

// The logged-in user's own account, for every role.
[Route("api/account")]
[Authorize]
public sealed class AccountController(ISender sender) : ApiController
{
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileCommand command, CancellationToken cancellationToken)
        => FromResult(await sender.Send(command, cancellationToken));

    [HttpPost("password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordCommand command, CancellationToken cancellationToken)
        => FromResult(await sender.Send(command, cancellationToken));
}
