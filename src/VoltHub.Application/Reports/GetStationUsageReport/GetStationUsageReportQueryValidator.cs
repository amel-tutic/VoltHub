using FluentValidation;

namespace VoltHub.Application.Reports.GetStationUsageReport;

internal sealed class GetStationUsageReportQueryValidator : AbstractValidator<GetStationUsageReportQuery>
{
    public GetStationUsageReportQueryValidator() => Include(new PeriodValidator());
}
