using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Users.CreateOperator;

// Operator accounts are never self-registered: only the administrator creates them (SSA process 1.5).
public sealed record CreateOperatorCommand(string FirstName, string LastName, string Email, string Password) : IRequest<Result<Guid>>;
