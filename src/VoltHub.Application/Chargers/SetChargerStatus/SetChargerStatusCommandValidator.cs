using FluentValidation;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Chargers.SetChargerStatus;

internal sealed class SetChargerStatusCommandValidator : AbstractValidator<SetChargerStatusCommand>
{
    public SetChargerStatusCommandValidator() =>
        RuleFor(x => x.Status)
            .Must(s => s is ChargerStatus.Available or ChargerStatus.OutOfOrder or ChargerStatus.UnderMaintenance)
            .WithMessage("Operators can set Available, OutOfOrder or UnderMaintenance; Occupied and Reserved are determined automatically.");
}
