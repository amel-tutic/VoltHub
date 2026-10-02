using MediatR;
using VoltHub.Application.Common.Results;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Stations.GetNearestStations;

// Recommendation: the closest stations with a charger available now (optionally of the vehicle's connector type).
public sealed record GetNearestStationsQuery(double Latitude, double Longitude, ConnectorType? ConnectorType, int Take)
    : IRequest<Result<List<NearestStationResponse>>>;
