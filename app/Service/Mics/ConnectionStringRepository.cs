using Microsoft.Data.SqlClient;
using Npgsql;

namespace Service;

public class ConnectionStringRepository(IConfiguration config)
{
    public static readonly string SqlServerConnConfigPath = "SqlServerConn";
    public static readonly string PostgresConfigPath = "PostgresConn";
    
    public SqlConnectionStringBuilder GetSqlServerConnBuilder()
    {
        return config.GetSqlServerConnBuilder(SqlServerConnConfigPath);
    }

    public string GetSqlServerConn()
    {
        return GetSqlServerConnBuilder().ConnectionString;
    }

    public NpgsqlConnectionStringBuilder GetPostgresConnBuilder()
    {
        return config.GetPostgresConnBuilder(PostgresConfigPath);
    }
}