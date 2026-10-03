using FluentValidation;

namespace VoltHub.Application.Reports.GetRevenueReport;

internal sealed class GetRevenueReportQueryValidator : AbstractValidator<GetRevenueReportQuery>
{
    public GetRevenueReportQueryValidator() => Include(new PeriodValidator());
}
