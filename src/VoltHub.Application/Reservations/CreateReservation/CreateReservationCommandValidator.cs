using FluentValidation;

namespace VoltHub.Application.Reservations.CreateReservation;

internal sealed class CreateReservationCommandValidator : AbstractValidator<CreateReservationCommand>
{
    public static readonly TimeSpan MaxDuration = TimeSpan.FromHours(8);

    public CreateReservationCommandValidator()
    {
        RuleFor(x => x.ChargerId).NotEmpty();
        RuleFor(x => x.VehicleId).NotEmpty();
        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time.")
            .Must((command, end) => end - command.StartTime <= MaxDuration)
            .WithMessage($"A reservation can last at most {MaxDuration.TotalHours} hours.");
    }
}
