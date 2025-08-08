using Microsoft.Extensions.Localization;
using TodoList.Interfaces;

namespace TodoList.Services;

public class I18nService(IStringLocalizerFactory factory) : II18nService
{
    private readonly IStringLocalizer _localizer = factory.Create(
        "Messages",
        typeof(Program).Assembly.GetName().Name ?? ""
    );

    public LocalizedString T(string key, params object[] arguments)
    {
        return _localizer[key, arguments];
    }
}
