using VoltHub.Application.Common.Interfaces;

namespace VoltHub.Api.Services;

// Reads the logged-in user from the validated JWT of the current HTTP request.
internal sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid? UserId =>
        Guid.TryParse(httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value, out var id) ? id : null;

    public bool IsInRole(string role) => httpContextAccessor.HttpContext?.User.IsInRole(role) ?? false;
}
