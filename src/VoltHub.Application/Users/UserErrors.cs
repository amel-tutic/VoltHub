using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Users;

internal static class UserErrors
{
    public static readonly Error NotFound = Error.NotFound("Users.NotFound", "User not found.");
    public static readonly Error EmailTaken = Error.Conflict("Users.EmailTaken", "An account with this email already exists.");
    public static readonly Error CannotDeactivateSelf = Error.Validation("Users.CannotDeactivateSelf",
        "You can't deactivate your own account.");
}
