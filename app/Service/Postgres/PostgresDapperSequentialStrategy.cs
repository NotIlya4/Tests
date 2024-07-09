using Dapper;
using Npgsql;
using Spam;

namespace Service;

public class PostgresDapperSequentialStrategy(NpgsqlConnection connection, ISimpleDataCreationStrategy<string> dataCreationStrategy) : ISpammerStrategy
{
    public async Task Execute(RunnerExecutionContext context, CancellationToken cancellationToken)
    {
        await connection.ExecuteAsync($"INSERT INTO \"SequentialEntities\" (\"SomeProperty\") VALUES (\'{dataCreationStrategy.CreateData()}\')");
    }
}