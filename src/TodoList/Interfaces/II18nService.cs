using Microsoft.Extensions.Localization;

namespace TodoList.Interfaces;

public interface II18nService
{
    public LocalizedString T(string key, params object[] arguments);
}
