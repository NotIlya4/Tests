using Dapper;
using Npgsql;
using Spam;

namespace Service;

public class PostgresDapperSelectStrategy(NpgsqlConnection connection, SelectStrategyType selectStrategyType, Random random, int count, int limit): ISpammerStrategy
{
    public async Task Execute(RunnerExecutionContext context, CancellationToken cancellationToken)
    {
        var id = random.Next(count);

        if (selectStrategyType == SelectStrategyType.RandomSingle)
        {
            await connection.ExecuteAsync($"SELECT * FROM \"SequentialEntities\" WHERE \"Id\" = {id}");
        }
        
        if (selectStrategyType == SelectStrategyType.All)
        {
            await connection.ExecuteAsync($"SELECT * FROM \"SequentialEntities\" LIMIT {limit}");
        }
    }
}