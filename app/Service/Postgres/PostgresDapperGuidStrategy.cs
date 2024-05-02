using Dapper;
using Npgsql;
using Spam;

namespace Service;

public class PostgresDapperGuidStrategy(PostgresDapperGuidStrategyOptions options) : ISpammerStrategy
{
    public async Task Prepare(int runnerIndex, Dictionary<object, object> runnerData, CancellationToken cancellationToken)
    {
        var connection = new NpgsqlConnection(options.Conn);
        runnerData["connection"] = connection;
        await connection.ExecuteAsync("SELECT * FROM \"StringEntities\" LIMIT 1");
    }

    public async Task Execute(RunnerExecutionContext context, CancellationToken cancellationToken)
    {
        var connection = (NpgsqlConnection)context.Data["connection"];
        await connection.ExecuteAsync($"INSERT INTO \"StringEntities\" (\"Id\") VALUES (\'{options.DataCreationStrategy.CreateData()}\')");
    }
}