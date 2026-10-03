using FluentValidation;

namespace VoltHub.Application.ProblemReports.CreateProblemReport;

internal sealed class CreateProblemReportCommandValidator : AbstractValidator<CreateProblemReportCommand>
{
    public CreateProblemReportCommandValidator()
    {
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
    }
}
