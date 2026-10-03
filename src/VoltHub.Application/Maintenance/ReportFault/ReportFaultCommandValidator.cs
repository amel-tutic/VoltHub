using FluentValidation;

namespace VoltHub.Application.Maintenance.ReportFault;

internal sealed class ReportFaultCommandValidator : AbstractValidator<ReportFaultCommand>
{
    public ReportFaultCommandValidator() => RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
}
