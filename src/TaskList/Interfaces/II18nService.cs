using Microsoft.Extensions.Localization;

namespace TaskList.Interfaces;

public interface II18nService
{
    public LocalizedString T(string key, params object[] arguments);
}
