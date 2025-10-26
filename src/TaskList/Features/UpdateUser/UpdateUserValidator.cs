using FluentValidation;
using TaskList.Interfaces;

namespace TaskList.Features.UpdateUser;

public class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserValidator(II18nService i18n)
    {
        RuleFor(x => x)
            .Must(x => x.NewPassword == x.NewPasswordConfirmation)
            .WithMessage(i18n.T("EquivalentPasswords"));

        RuleFor(x => x.Name).Length(2, 64).WithMessage(i18n.T("FieldLengthBetween", 2, 64));

        RuleFor(x => x.Username)
            .Length(4, 32)
            .WithMessage(i18n.T("FieldLengthBetween", 4, 32))
            .Matches(@"^[a-zA-Z0-9_.-]+$")
            .WithMessage(i18n.T("UsernameMustBeValid"));

        RuleFor(x => x.Email)
            .Length(8, 64)
            .WithMessage(i18n.T("FieldLengthBetween", 8, 64))
            .EmailAddress()
            .WithMessage(i18n.T("EmailMustBeValid"));

        RuleFor(x => x.Password).Length(8, 64).WithMessage(i18n.T("FieldLengthBetween", 8, 64));

        RuleFor(x => x.NewPassword).Length(8, 64).WithMessage(i18n.T("FieldLengthBetween", 8, 64));

        RuleFor(x => x.NewPasswordConfirmation)
            .Length(8, 64)
            .WithMessage(i18n.T("FieldLengthBetween", 8, 64));
    }
}
