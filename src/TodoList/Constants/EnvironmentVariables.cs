using TodoList.Utils;

namespace TodoList.Constants;

public static class EnvironmentVariables
{
    // Client
    public static string ClientUrl => EnvUtil.GetVar("CLIENT_URL", "");

    // Database
    public static string DatabaseHost => EnvUtil.GetVar("DATABASE_HOST", "");
    public static string DatabaseName => EnvUtil.GetVar("DATABASE_NAME", "");
    public static string DatabasePassword => EnvUtil.GetVar("DATABASE_PASSWORD", "");
    public static int DatabasePort => EnvUtil.GetVar("DATABASE_PORT", 0);
    public static string DatabaseUser => EnvUtil.GetVar("DATABASE_USER", "");

    // Mail
    public static string MailHost => EnvUtil.GetVar("MAIL_HOST", "");
    public static string MailPassword => EnvUtil.GetVar("MAIL_PASSWORD", "");
    public static int MailPort => EnvUtil.GetVar("MAIL_PORT", 0);
    public static string MailSender => EnvUtil.GetVar("MAIL_SENDER", "");
    public static string MailUsername => EnvUtil.GetVar("MAIL_USERNAME", "");

    // JWT
    public static string JwtAudience => EnvUtil.GetVar("JWT_AUDIENCE", "");
    public static string JwtIssuer => EnvUtil.GetVar("JWT_ISSUER", "");
    public static string JwtSecret => EnvUtil.GetVar("JWT_SECRET", "");
}
