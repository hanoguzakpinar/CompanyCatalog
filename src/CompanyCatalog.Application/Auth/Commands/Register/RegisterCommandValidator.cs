using FluentValidation;

namespace CompanyCatalog.Application.Auth.Commands.Register;

internal sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);

        RuleFor(c => c.Password)
            .NotEmpty()
            .MaximumLength(100)
            .MinimumLength(8)
            .Matches(@"[A-Z]").WithMessage(
                "Şifre en az bir büyük karakter içermelidir.")
            .Matches(@"[a-z]").WithMessage(
                "Şifre en az bir küçük karakter içermelidir.")
            .Matches(@"[0-9]").WithMessage(
                "Şifre en az bir sayı içermelidir.");

        RuleFor(c => c.FullName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(100);
    }
}