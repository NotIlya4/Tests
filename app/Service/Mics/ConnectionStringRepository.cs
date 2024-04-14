using Microsoft.Data.SqlClient;

namespace Service;

public class ConnectionStringRepository(IConfiguration config)
{
    public static readonly string SqlServerConnConfigPath = "SqlServerConn";
    
    public SqlConnectionStringBuilder GetSqlServerConnBuilder()
    {
        return config.GetSqlServerConnBuilder(SqlServerConnConfigPath);
    }

    public string GetSqlServerConn()
    {
        return GetSqlServerConnBuilder().ConnectionString;
    }
}