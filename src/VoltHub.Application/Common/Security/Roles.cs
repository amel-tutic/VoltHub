using VoltHub.Domain.Enums;

namespace VoltHub.Application.Common.Security;

// Role names as constants, derived from the enum so they can never drift apart.
public static class Roles
{
    public const string User = nameof(UserRole.User);
    public const string Operator = nameof(UserRole.Operator);
    public const string Admin = nameof(UserRole.Admin);
    public const string Staff = Operator + "," + Admin;   // [Authorize(Roles = Roles.Staff)] = operator OR admin
}
