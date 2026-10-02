using FluentValidation;

namespace VoltHub.Application.Reservations.GetChargerSchedule;

internal sealed class GetChargerScheduleQueryValidator : AbstractValidator<GetChargerScheduleQuery>
{
    public GetChargerScheduleQueryValidator()
    {
        When(x => x.From.HasValue && x.To.HasValue, () =>
        {
            RuleFor(x => x.To)
                .GreaterThan(x => x.From).WithMessage("'To' must be after 'From'.")
                .Must((query, to) => to!.Value - query.From!.Value <= TimeSpan.FromDays(31))
                .WithMessage("The window can span at most 31 days.");
        });
    }
}
