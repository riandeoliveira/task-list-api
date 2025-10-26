using FluentValidation;
using TaskList.Interfaces;

namespace TaskList.Features.SignInUser;

public class SignInUserValidator : AbstractValidator<SignInUserRequest>
{
    public SignInUserValidator(II18nService i18n)
    {
        RuleFor(x => x.UsernameOrEmail).NotEmpty().WithMessage(i18n.T("FieldIsRequired"));

        RuleFor(x => x.UsernameOrEmail)
            .EmailAddress()
            .WithMessage(i18n.T("EmailMustBeValid"))
            .Length(8, 64)
            .WithMessage(i18n.T("FieldLengthBetween", 8, 64))
            .When(x => !string.IsNullOrEmpty(x.UsernameOrEmail) && x.UsernameOrEmail.Contains('@'));

        RuleFor(x => x.UsernameOrEmail)
            .Matches("^[a-zA-Z0-9_.-]+$")
            .WithMessage(i18n.T("UsernameMustBeValid"))
            .Length(4, 32)
            .WithMessage(i18n.T("FieldLengthBetween", 4, 32))
            .Unless(x =>
                !string.IsNullOrEmpty(x.UsernameOrEmail) && x.UsernameOrEmail.Contains('@')
            );

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(i18n.T("FieldIsRequired"))
            .Length(8, 64)
            .WithMessage(i18n.T("FieldLengthBetween", 8, 64));
    }
}
