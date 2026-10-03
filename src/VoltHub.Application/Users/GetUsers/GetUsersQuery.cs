using MediatR;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Users.GetUsers;

// SSA process 1.5: the administrator's list of every account, optionally filtered.
public sealed record GetUsersQuery(UserRole? Role, bool? IsActive, string? Search) : IRequest<Result<List<UserAdminResponse>>>;
