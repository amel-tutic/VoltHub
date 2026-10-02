using FluentValidation;

namespace VoltHub.Application.Vehicles.UpdateVehicle;

internal sealed class UpdateVehicleCommandValidator : AbstractValidator<UpdateVehicleCommand>
{
    public UpdateVehicleCommandValidator()
    {
        RuleFor(x => x.Make).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Model).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BatteryCapacityKwh).GreaterThan(0).LessThanOrEqualTo(1000m);
        RuleFor(x => x.ConnectorType).IsInEnum();
    }
}
