using Spam;

namespace Service.Enums;

public static class DataCreationStrategyTypeExtensions
{
    public static ISimpleDataCreationStrategy<string> CreateStrategy(this DataCreationStrategyType type, int fixedLengthStringLength)
    {
        return type switch
        {
            DataCreationStrategyType.Guid => new GuidDataCreationStrategy(),
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
    
    public static ISpammerStrategy CreateStrategy(this PostgresStrategyType type, string conn, ISimpleDataCreationStrategy<string> dataCreationStrategy, int throttleMs)
    {
        return type switch
        {
            PostgresStrategyType.DapperInsertSeqEntity => new PostgresDapperSequentialStrategy(conn, dataCreationStrategy),
            PostgresStrategyType.DapperInsertStringEntity => new PostgresDapperGuidStrategy(new PostgresDapperGuidStrategyOptions()
            {
                Conn = conn,
                DataCreationStrategy = dataCreationStrategy
            }),
            PostgresStrategyType.DapperSelect => new PostgresDapperSelectStrategy(conn, throttleMs),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}