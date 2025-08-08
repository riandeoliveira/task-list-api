using System.Globalization;

namespace TodoList.Utils;

public static class EnvUtil
{
    public static T GetVar<T>(string key, T fallback)
    {
        var value = Environment.GetEnvironmentVariable(key);

        if (string.IsNullOrWhiteSpace(value))
        {
            return fallback;
        }

        try
        {
            var targetType = typeof(T);

            if (targetType.IsEnum)
            {
                var parsed = Enum.TryParse(
                    targetType,
                    value,
                    ignoreCase: true,
                    out object? enumResult
                );

                return parsed ? (T)enumResult! : fallback;
            }

            if (targetType == typeof(Guid))
            {
                var parsed = Guid.TryParse(value, out var guidResult);

                return parsed ? (T)(object)guidResult : fallback;
            }

            if (targetType == typeof(bool))
            {
                var parsed = bool.TryParse(value, out var boolResult);

                return parsed ? (T)(object)boolResult : fallback;
            }

            if (targetType == typeof(int))
            {
                var parsed = int.TryParse(
                    value,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out int intResult
                );

                return parsed ? (T)(object)intResult : fallback;
            }

            return (T)(object)value;
        }
        catch
        {
            return fallback;
        }
    }
}
