using VoltHub.Domain.Enums;

namespace VoltHub.Application.Users;

public sealed record UserAdminResponse(
    Guid Id, string FirstName, string LastName, string Email, UserRole Role, bool IsActive, DateTime CreatedAt, int VehicleCount);
