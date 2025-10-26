using FluentValidation;
using TaskList.Interfaces;

namespace TaskList.Features.ResetUserPassword;

public class ResetUserPasswordValidator : AbstractValidator<ResetUserPasswordRequest>
{
    public ResetUserPasswordValidator(II18nService i18n)
    {
        RuleFor(x => x)
            .Must(x => x.Password == x.PasswordConfirmation)
            .WithMessage(i18n.T("EquivalentPasswords"));

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

        RuleFor(x => x.Token).NotEmpty().WithMessage(i18n.T("FieldIsRequired"));
    }
}
