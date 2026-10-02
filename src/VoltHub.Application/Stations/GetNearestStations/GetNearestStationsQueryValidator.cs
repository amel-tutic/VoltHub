using FluentValidation;

namespace VoltHub.Application.Stations.GetNearestStations;

internal sealed class GetNearestStationsQueryValidator : AbstractValidator<GetNearestStationsQuery>
{
    public GetNearestStationsQueryValidator()
    {
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
        RuleFor(x => x.Take).InclusiveBetween(1, 20);
    }
}
