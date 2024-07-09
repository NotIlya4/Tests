using Dapper;
using Npgsql;
using Spam;

namespace Service;

public class PostgresDapperGuidStrategy(NpgsqlConnection connection, ISimpleDataCreationStrategy<string> dataCreationStrategy) : ISpammerStrategy
{
    public async Task Execute(RunnerExecutionContext context, CancellationToken cancellationToken)
    {
        await connection.ExecuteAsync($"INSERT INTO \"StringEntities\" (\"Id\") VALUES (\'{dataCreationStrategy.CreateData()}\')");
    }
}