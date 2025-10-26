using TaskList.Utils;

namespace TaskList.Constants;

public static class EnvironmentVariables
{
    // App
    public static int AppPort => EnvUtil.GetVar("APP_PORT", 5146);

    // Client
    public static string ClientUrl => EnvUtil.GetVar("CLIENT_URL", "http://localhost:5173");

    // Database
    public static string DatabaseHost => EnvUtil.GetVar("DATABASE_HOST", "localhost");
    public static string DatabaseName => EnvUtil.GetVar("DATABASE_NAME", "task_list_db");
    public static string DatabasePassword => EnvUtil.GetVar("DATABASE_PASSWORD", "root");
    public static int DatabasePort => EnvUtil.GetVar("DATABASE_PORT", 5432);
    public static string DatabaseUser => EnvUtil.GetVar("DATABASE_USER", "root");

    // Mail
    public static string MailHost => EnvUtil.GetVar("MAIL_HOST", "");
    public static string MailPassword => EnvUtil.GetVar("MAIL_PASSWORD", "");
    public static int MailPort => EnvUtil.GetVar("MAIL_PORT", 0);
    public static string MailSender => EnvUtil.GetVar("MAIL_SENDER", "");
    public static string MailUsername => EnvUtil.GetVar("MAIL_USERNAME", "");

    // JWT
    public static string JwtAudience => EnvUtil.GetVar("JWT_AUDIENCE", "http://localhost:5146");
    public static string JwtIssuer => EnvUtil.GetVar("JWT_ISSUER", "http://localhost:5146");
    public static string JwtSecret => EnvUtil.GetVar("JWT_SECRET", "");
}
