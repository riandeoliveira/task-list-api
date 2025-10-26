namespace TaskList.Constants;

public static class DatabaseAccess
{
    public static string ConnectionString =>
        $@"
            Server={EnvironmentVariables.DatabaseHost};
            Port={EnvironmentVariables.DatabasePort};
            Database={EnvironmentVariables.DatabaseName};
            Username={EnvironmentVariables.DatabaseUser};
            Password={EnvironmentVariables.DatabasePassword}
        ";
}
