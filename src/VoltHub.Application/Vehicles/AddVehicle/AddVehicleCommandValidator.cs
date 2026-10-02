using FluentValidation;

namespace VoltHub.Application.Vehicles.AddVehicle;

internal sealed class AddVehicleCommandValidator : AbstractValidator<AddVehicleCommand>
{
    public AddVehicleCommandValidator()
    {
        RuleFor(x => x.Make).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Model).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BatteryCapacityKwh).GreaterThan(0).LessThanOrEqualTo(1000m);
        RuleFor(x => x.ConnectorType).IsInEnum();
    }
}
