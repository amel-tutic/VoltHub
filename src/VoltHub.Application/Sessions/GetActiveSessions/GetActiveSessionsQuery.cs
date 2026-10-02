using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Sessions.GetActiveSessions;

// SSA process 4.2: the live view of the caller's running charging sessions.
public sealed record GetActiveSessionsQuery : IRequest<Result<List<ActiveSessionResponse>>>;
