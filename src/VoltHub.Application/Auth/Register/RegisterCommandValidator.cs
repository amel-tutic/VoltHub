using FluentValidation;

namespace VoltHub.Application.Auth.Register;

internal sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(72)   // BCrypt only uses the first 72 bytes of a password
            .Matches("[A-Za-z]").WithMessage("Password must contain a letter.")
            .Matches("[0-9]").WithMessage("Password must contain a digit.");
    }
}
