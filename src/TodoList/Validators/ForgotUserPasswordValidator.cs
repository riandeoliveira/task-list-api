using FluentValidation;
using TodoList.Interfaces;
using TodoList.Requests;

namespace TodoList.Validators;

public class ForgotUserPasswordValidator : AbstractValidator<ForgotUserPasswordRequest>
{
    public ForgotUserPasswordValidator(II18nService i18n)
    {
        RuleFor(x => x.UsernameOrEmail).NotEmpty().WithMessage(i18n.T("FieldIsRequired"));

        RuleFor(x => x.UsernameOrEmail)
            .EmailAddress()
            .WithMessage(i18n.T("EmailIsValid"))
            .Length(8, 64)
            .WithMessage(i18n.T("FieldLengthBetween", 8, 64))
            .When(x => x.UsernameOrEmail.Contains('@'));

        RuleFor(x => x.UsernameOrEmail)
            .Matches("^[a-zA-Z0-9_.-]+$")
            .WithMessage(i18n.T("UsernameIsValid"))
            .Length(4, 32)
            .WithMessage(i18n.T("FieldLengthBetween", 4, 32))
            .Unless(x => x.UsernameOrEmail.Contains('@'));
    }
}
