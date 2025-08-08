using FluentValidation;
using TodoList.Interfaces;
using TodoList.Requests;

namespace TodoList.Validators;

public class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserValidator(II18nService i18n)
    {
        RuleFor(x => x.Name).Length(2, 64).WithMessage(i18n.T("FieldLengthBetween", 2, 64));

        RuleFor(x => x.Username)
            .Length(4, 32)
            .WithMessage(i18n.T("FieldLengthBetween", 4, 32))
            .Matches(@"^[a-zA-Z0-9_.-]+$")
            .WithMessage(i18n.T("UsernameIsValid"));

        RuleFor(x => x.Email)
            .Length(8, 64)
            .WithMessage(i18n.T("FieldLengthBetween", 8, 64))
            .EmailAddress()
            .WithMessage(i18n.T("EmailIsValid"));

        RuleFor(x => x.Password).Length(8, 64).WithMessage(i18n.T("FieldLengthBetween", 8, 64));
    }
}
