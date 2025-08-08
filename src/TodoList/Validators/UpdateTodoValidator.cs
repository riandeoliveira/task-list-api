using FluentValidation;
using TodoList.Interfaces;
using TodoList.Requests;

namespace TodoList.Validators;

public class UpdateTodoValidator : AbstractValidator<UpdateTodoRequest>
{
    public UpdateTodoValidator(II18nService i18n)
    {
        RuleFor(x => x.Title).Length(4, 128).WithMessage(i18n.T("FieldLengthBetween", 4, 128));

        RuleFor(x => x.Description).MaximumLength(1024).WithMessage(i18n.T("FieldMaxLength", 1024));
    }
}
