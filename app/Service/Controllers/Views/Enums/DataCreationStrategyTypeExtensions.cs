﻿using Dapper;
using Npgsql;
using Spam;

namespace Service.Enums;

public static class DataCreationStrategyTypeExtensions
{
    public static ISimpleDataCreationStrategy<string> CreateStrategy(this DataCreationStrategyType type, int fixedLengthStringLength)
    {
        return type switch
        {
            DataCreationStrategyType.Guid => new GuidDataCreationStrategy(),
            DataCreationStrategyType.SeqGuid => new SeqGuidDataCreationStrategy(),
            DataCreationStrategyType.FixedLengthString => new FixedLengthStringDataCreationStrategy(
                fixedLengthStringLength),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
    
    public static ISpammerParallelEngine CreateEngine(this SpammerParallelEngineType type)
    {
        return type switch
        {
            SpammerParallelEngineType.ParallelForEachAsync => new ParallelForAsyncParallelEngine(),
            SpammerParallelEngineType.For => new ForParallelEngine(),
            SpammerParallelEngineType.Threads => new ThreadsParallelEngine(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public static IDbContextOperationStrategy CreateDbContextStrategy(this DbContextStrategyType type, ISimpleDataCreationStrategy<string> dataCreationStrategy)
    {
        return type switch
        {
            DbContextStrategyType.SequentialEntity => new SequentialEntityInsertStrategy(new SequentialEntityInsertStrategyOptions()
            {
                DataCreationStrategy = dataCreationStrategy
            }),
            DbContextStrategyType.StringEntity => new StringEntityInsertStrategy(new StringEntityInsertStrategyOptions()
            {
                DataCreationStrategy = dataCreationStrategy
            }),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
    
    public static async Task<ISpammerStrategy> CreateStrategy(
        this PostgresStrategyType type,
        string conn,
        ISimpleDataCreationStrategy<string> dataCreationStrategy,
        SelectStrategyType selectStrategyType,
        int limit)
    {
        var connection = new NpgsqlConnection(conn);
        await connection.ExecuteAsync("SELECT * FROM \"StringEntities\" LIMIT 1");

        return type switch
        {
            PostgresStrategyType.DapperInsertSeqEntity => new PostgresDapperSequentialStrategy(connection,
                dataCreationStrategy),
            PostgresStrategyType.DapperInsertStringEntity => new PostgresDapperGuidStrategy(connection,
                dataCreationStrategy),
            PostgresStrategyType.DapperSelect => new PostgresDapperSelectStrategy(connection, selectStrategyType,
                new Random(Random.Shared.Next()),
                await connection.QueryFirstAsync<int>("SELECT count(*) FROM \"SequentialEntities\";"), limit),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}