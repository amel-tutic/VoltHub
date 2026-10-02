using MediatR;
using VoltHub.Application.Common.Results;

namespace VoltHub.Application.Stations.GetStationById;

public sealed record GetStationByIdQuery(Guid Id) : IRequest<Result<StationDetailsResponse>>;
