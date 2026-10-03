using FluentValidation;

namespace VoltHub.Application.Ratings.RateStation;

internal sealed class RateStationCommandValidator : AbstractValidator<RateStationCommand>
{
    public RateStationCommandValidator()
    {
        RuleFor(x => x.Score).InclusiveBetween(1, 5);
        RuleFor(x => x.Comment).MaximumLength(1000);
    }
}
