using FluentValidation;

namespace VoltHub.Application.Reports;

// Shared by the period-based reports: the last 30 days unless the administrator chooses otherwise.
internal static class ReportPeriod
{
    public static (DateTime From, DateTime To) Resolve(DateTimeOffset? from, DateTimeOffset? to)
    {
        var end = to?.UtcDateTime ?? DateTime.UtcNow;
        var start = from?.UtcDateTime ?? end.AddDays(-30);
        return (start, end);
    }
}

internal interface IPeriodQuery
{
    DateTimeOffset? From { get; }
    DateTimeOffset? To { get; }
}

internal sealed class PeriodValidator : AbstractValidator<IPeriodQuery>
{
    public PeriodValidator()
    {
        When(x => x.From.HasValue && x.To.HasValue, () =>
        {
            RuleFor(x => x.To)
                .GreaterThan(x => x.From).WithMessage("'To' must be after 'From'.")
                .Must((query, to) => to!.Value - query.From!.Value <= TimeSpan.FromDays(366))
                .WithMessage("A report can cover at most one year.");
        });
    }
}
