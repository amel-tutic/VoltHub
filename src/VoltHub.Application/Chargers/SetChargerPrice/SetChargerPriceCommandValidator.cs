using FluentValidation;

namespace VoltHub.Application.Chargers.SetChargerPrice;

internal sealed class SetChargerPriceCommandValidator : AbstractValidator<SetChargerPriceCommand>
{
    public SetChargerPriceCommandValidator() =>
        RuleFor(x => x.PricePerKwh).GreaterThanOrEqualTo(0).LessThanOrEqualTo(1000m);
}
