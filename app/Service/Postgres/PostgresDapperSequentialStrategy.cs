using Dapper;
using Npgsql;
using Spam;

namespace Service;

public class PostgresDapperSequentialStrategy(string conn, ISimpleDataCreationStrategy<string> dataCreationStrategy) : ISpammerStrategy
{
    public async Task Prepare(int runnerIndex, Dictionary<object, object> runnerData, CancellationToken cancellationToken)
    {
        var connection = new NpgsqlConnection(conn);
        runnerData["connection"] = connection;
        await connection.ExecuteAsync("SELECT * FROM \"SequentialEntities\" LIMIT 1");
    }

    public async Task Execute(RunnerExecutionContext context, CancellationToken cancellationToken)
    {
        var connection = (NpgsqlConnection)context.Data["connection"];
        await connection.ExecuteAsync($"INSERT INTO \"SequentialEntities\" (\"SomeProperty\") VALUES (\'{dataCreationStrategy.CreateData()}\')");
    }
}