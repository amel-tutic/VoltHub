using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Statistics.GetMyStatistics;

// SSA process 8.2: totals, favourite stations and the last six months, from completed sessions.
public sealed record GetMyStatisticsQuery : IRequest<Result<MyStatisticsResponse>>;
