using FluentValidation;
using TaskList.Interfaces;

namespace TaskList.Features.CreateTask;

public class CreateTaskValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskValidator(II18nService i18n)
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(i18n.T("FieldIsRequired"))
            .Length(4, 128)
            .WithMessage(i18n.T("FieldLengthBetween", 4, 128));

        RuleFor(x => x.Description).MaximumLength(1024).WithMessage(i18n.T("FieldMaxLength", 1024));
    }
}
