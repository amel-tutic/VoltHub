using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Account;

internal static class AccountErrors
{
    public static readonly Error UserNotFound = Error.NotFound("Account.UserNotFound", "User not found.");

    // 400, not 401: a 401 tells the web client its login expired, and it would try to refresh the token.
    public static readonly Error WrongCurrentPassword = Error.Validation("Account.WrongCurrentPassword", "The current password is not correct.");
}
