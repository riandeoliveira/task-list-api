using FluentValidation;
using TaskList.Interfaces;

namespace TaskList.Features.UpdateTask;

public class UpdateTaskValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskValidator(II18nService i18n)
    {
        RuleFor(x => x.Title).Length(4, 128).WithMessage(i18n.T("FieldLengthBetween", 4, 128));

        RuleFor(x => x.Description).MaximumLength(1024).WithMessage(i18n.T("FieldMaxLength", 1024));
    }
}
