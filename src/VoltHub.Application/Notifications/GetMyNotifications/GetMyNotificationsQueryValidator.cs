using FluentValidation;

namespace VoltHub.Application.Notifications.GetMyNotifications;

internal sealed class GetMyNotificationsQueryValidator : AbstractValidator<GetMyNotificationsQuery>
{
    public GetMyNotificationsQueryValidator() => RuleFor(x => x.Take).InclusiveBetween(1, 200);
}
