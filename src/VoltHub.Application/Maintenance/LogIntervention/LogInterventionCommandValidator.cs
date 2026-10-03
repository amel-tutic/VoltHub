using FluentValidation;
using VoltHub.Domain.Enums;

namespace VoltHub.Application.Maintenance.LogIntervention;

internal sealed class LogInterventionCommandValidator : AbstractValidator<LogInterventionCommand>
{
    public LogInterventionCommandValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Type)
            .Must(type => type is MaintenanceType.Repair or MaintenanceType.Inspection)
            .WithMessage("An intervention is a Repair or an Inspection. Report faults and plan services separately.");
    }
}
