using FluentValidation;

namespace VoltHub.Application.Chargers.AddCharger;

internal sealed class AddChargerCommandValidator : AbstractValidator<AddChargerCommand>
{
    public AddChargerCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ConnectorType).IsInEnum();
        RuleFor(x => x.CurrentType).IsInEnum();
        RuleFor(x => x.PowerKw).GreaterThan(0).LessThanOrEqualTo(9999.99m);         // NUMERIC(6,2)
        RuleFor(x => x.PricePerKwh).GreaterThanOrEqualTo(0).LessThanOrEqualTo(1000m);
    }
}
