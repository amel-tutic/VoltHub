using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Sessions.GetMySessions;

// SSA process 8.1: the caller's charging history.
public sealed record GetMySessionsQuery : IRequest<Result<List<SessionHistoryResponse>>>;
