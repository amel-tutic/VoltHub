using FluentValidation;

namespace VoltHub.Application.Account.ChangePassword;

internal sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(72)   // BCrypt only uses the first 72 bytes of a password
            .Matches("[A-Za-z]").WithMessage("Password must contain a letter.")
            .Matches("[0-9]").WithMessage("Password must contain a digit.")
            .NotEqual(x => x.CurrentPassword).WithMessage("The new password must be different from the current one.");
    }
}
