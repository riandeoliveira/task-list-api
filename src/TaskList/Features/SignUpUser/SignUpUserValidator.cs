using FluentValidation;
using TaskList.Interfaces;

namespace TaskList.Features.SignUpUser;

public class SignUpUserValidator : AbstractValidator<SignUpUserRequest>
{
    public SignUpUserValidator(II18nService i18n)
    {
        RuleFor(x => x)
            .Must(x => x.Password == x.PasswordConfirmation)
            .WithMessage(i18n.T("EquivalentPasswords"));

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(i18n.T("FieldIsRequired"))
            .Length(2, 64)
            .WithMessage(i18n.T("FieldLengthBetween", 2, 64));

        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage(i18n.T("FieldIsRequired"))
            .Length(4, 32)
            .WithMessage(i18n.T("FieldLengthBetween", 4, 32))
            .Matches(@"^[a-zA-Z0-9_.-]+$")
            .WithMessage(i18n.T("UsernameMustBeValid"));

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(i18n.T("FieldIsRequired"))
            .Length(8, 64)
            .WithMessage(i18n.T("FieldLengthBetween", 8, 64))
            .EmailAddress()
            .WithMessage(i18n.T("EmailMustBeValid"));

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(i18n.T("FieldIsRequired"))
            .Length(8, 64)
            .WithMessage(i18n.T("FieldLengthBetween", 8, 64));

        RuleFor(x => x.PasswordConfirmation)
            .NotEmpty()
            .WithMessage(i18n.T("FieldIsRequired"))
            .Length(8, 64)
            .WithMessage(i18n.T("FieldLengthBetween", 8, 64));
    }
}
