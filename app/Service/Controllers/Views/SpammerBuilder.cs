using Microsoft.EntityFrameworkCore;
using Service.Enums;
using Spam;

namespace Service;

public class SpammerBuilder(
    ILogger<Spammer> logger,
    AppMetrics appMetrics,
    NginxPingServiceFactory nginxPingServiceFactory)
{
    private int? _runnerExecutions;
    private int? _parallelRunners;
    private string? _testName;
    private ISpammerStrategy? _spammerStrategy;
    private ISpammerParallelEngine? _spammerParallelEngine;

    public SpammerBuilder WithDbContextStrategy(
        IDbContextFactory<AppDbContext> dbContextFactory,
        DbContextStrategyType dbContextStrategyType,
        DataCreationStrategyType dataCreationStrategyType,
        int fixedLengthStringLength)
    {
        _spammerStrategy = new DbContextSpammerStrategy(new DbContextSpammerStrategyOptions()
        {
            DbContextFactory = dbContextFactory,
            DbContextRetryStrategy = DbContextRetryStrategy.FullExpensive,
            HotConnections = true,
            OperationStrategy = dbContextStrategyType.CreateDbContextStrategy(dataCreationStrategyType.CreateStrategy(fixedLengthStringLength))
        });
        
        return this;
    }

    public SpammerBuilder WithNginxStrategy(NginxPingMode pingMode)
    {
        _spammerStrategy = new NginxStrategy(new NginxSpammerStrategyOptions()
        {
            NginxPingMode = pingMode,
            PingServiceFactory = nginxPingServiceFactory
        });

        return this;
    }
    
    public SpammerBuilder WithRunnerExecutions(int runnerExecutions)
    {
        _runnerExecutions = runnerExecutions;
        return this;
    }
    
    public SpammerBuilder WithSpammerStrategy(ISpammerStrategy spammerStrategy)
    {
        _spammerStrategy = spammerStrategy;
        return this;
    }
    
    public SpammerBuilder WithParallelEngine(ISpammerParallelEngine spammerParallelEngine)
    {
        _spammerParallelEngine = spammerParallelEngine;
        return this;
    }

    public SpammerBuilder WithParallelRunners(int parallelRunners)
    {
        _parallelRunners = parallelRunners;
        return this;
    }
    
    public SpammerBuilder WithTestName(string testName)
    {
        _testName = testName;
        return this;
    }

    public SpammerBuilder ApplyView(SpammerViewBase viewBase)
    {
        _parallelRunners = viewBase.ParallelRunners;
        _runnerExecutions = viewBase.RunnerExecutions;
        _testName = viewBase.TestName;
        _spammerParallelEngine = viewBase.SpammerParallelEngineType.CreateEngine();
        return this;
    }

    protected virtual Spammer Create(SpammerOptions spammerOptions)
    {
        return new Spammer(spammerOptions);
    }

    public Spammer Build()
    {
        ArgumentNullException.ThrowIfNull(_parallelRunners);
        ArgumentNullException.ThrowIfNull(_runnerExecutions);
        ArgumentNullException.ThrowIfNull(_testName);
        ArgumentNullException.ThrowIfNull(_spammerStrategy);
        ArgumentNullException.ThrowIfNull(_spammerParallelEngine);

        return Create(new SpammerOptions()
        {
            Logger = logger,
            SpammerStrategy = _spammerStrategy,
            SpammerParallelEngine = _spammerParallelEngine,
            Metrics = new SpammerMetrics(appMetrics, _testName, nameof(Spammer)),
            ParallelRunners = _parallelRunners.Value,
            RunnerExecutions = _runnerExecutions.Value
        });
    }
}