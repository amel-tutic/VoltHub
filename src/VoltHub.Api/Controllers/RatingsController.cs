using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoltHub.Api.Contracts;
using VoltHub.Application.Common.Security;
using VoltHub.Application.Ratings.GetStationRatings;
using VoltHub.Application.Ratings.RateStation;

namespace VoltHub.Api.Controllers;

[Route("api/stations/{stationId:guid}/ratings")]
[Authorize]
public sealed class RatingsController(ISender sender) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> Get(Guid stationId, CancellationToken cancellationToken)
        => FromResult(await sender.Send(new GetStationRatingsQuery(stationId), cancellationToken));

    [HttpPut]
    [Authorize(Roles = Roles.User)]
    public async Task<IActionResult> Rate(Guid stationId, RatingRequest request, CancellationToken cancellationToken)
        => FromResult(await sender.Send(new RateStationCommand(stationId, request.Score, request.Comment), cancellationToken));
}
