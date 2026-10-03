using FluentValidation;

namespace VoltHub.Application.Reports.GetFaultReport;

internal sealed class GetFaultReportQueryValidator : AbstractValidator<GetFaultReportQuery>
{
    public GetFaultReportQueryValidator() => Include(new PeriodValidator());
}
