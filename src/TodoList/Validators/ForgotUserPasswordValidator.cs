using FluentValidation;
using TodoList.Interfaces;
using TodoList.Requests;

namespace TodoList.Validators;

public class ForgotUserPasswordValidator : AbstractValidator<ForgotUserPasswordRequest>
{
    public ForgotUserPasswordValidator(II18nService i18n)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(i18n.T("FieldIsRequired"))
            .Length(8, 64)
            .WithMessage(i18n.T("FieldLengthBetween", 8, 64))
            .EmailAddress()
            .WithMessage(i18n.T("EmailIsValid"));
    }
}
