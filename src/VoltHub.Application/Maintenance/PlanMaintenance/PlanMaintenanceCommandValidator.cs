using FluentValidation;

namespace VoltHub.Application.Maintenance.PlanMaintenance;

internal sealed class PlanMaintenanceCommandValidator : AbstractValidator<PlanMaintenanceCommand>
{
    public PlanMaintenanceCommandValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.ScheduledDate)
            .Must(date => date > DateTimeOffset.UtcNow).WithMessage("The service must be planned for a future date.")
            .Must(date => date < DateTimeOffset.UtcNow.AddYears(2)).WithMessage("Plan services at most two years ahead.");
    }
}
