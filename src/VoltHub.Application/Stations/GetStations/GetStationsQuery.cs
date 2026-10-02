using MediatR;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Stations.GetStations;

// All filters are optional; null means "don't filter on this".
public sealed record GetStationsQuery(
    string? City, ConnectorType? ConnectorType, CurrentType? CurrentType, decimal? MinPowerKw, bool OnlyAvailable)
    : IRequest<Result<List<StationSummaryResponse>>>;
