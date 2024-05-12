using Dapper;
using Npgsql;
using Spam;

namespace Service;

public class PostgresDapperSelectStrategy(
    string conn,
    int throttleMs): ISpammerStrategy
{
    public async Task Prepare(int runnerIndex, Dictionary<object, object> runnerData, CancellationToken cancellationToken)
    {
        var connection = new NpgsqlConnection(conn);
        runnerData["connection"] = connection;
        runnerData["random"] = new Random(Random.Shared.Next());
        runnerData["count"] = await connection.QueryFirstAsync<int>("SELECT count(*) FROM \"SequentialEntities\";");
    }

    public async Task Execute(RunnerExecutionContext context, CancellationToken cancellationToken)
    {
        await Task.Delay(throttleMs, cancellationToken);
        var id = ((Random)context.Data["random"]).Next((int)context.Data["count"]);
        var connection = (NpgsqlConnection)context.Data["connection"];
        await connection.ExecuteAsync($"SELECT * FROM \"SequentialEntities\" WHERE \"Id\" = {id}");
    }
}