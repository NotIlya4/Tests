using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Service.Enums;
using Spam;

namespace Service;

public class SpammerBuilder(
    ILogger<Spammer> logger,
    AppMetrics appMetrics,
    IOptions<KafkaOptions> kafkaOptions,
    IServiceProvider serviceProvider)
{
    private int? _runnerExecutions;
    private int? _parallelRunners;
    private string? _testName;
    private Func<CancellationToken, Task<ISpammerStrategy>>? _spammerStrategyFactory;
    private ISpammerParallelEngine? _spammerParallelEngine;

    public SpammerBuilder WithKafkaProducerStrategy(KafkaProducerStrategyOptionsView optionsView)
    {
        IProducer<string, string> CreateProducer()
        {
            var producerConfig = new ProducerConfig()
            {
                BootstrapServers = kafkaOptions.Value.BootstrapServers,
                CompressionType = optionsView.CompressionType,
                EnableIdempotence = optionsView.EnableIdempotence,
                Acks = optionsView.Acks,
                ClientId = $"{optionsView.SpammerOptionsView.TestName}-client",
                MessageMaxBytes = optionsView.MaxMsgSize == -1 ? optionsView.Size + 100 : optionsView.MaxMsgSize,
                SocketNagleDisable = optionsView.SocketNagleDisable,
                MaxInFlight = optionsView.MaxInFlight,
                LingerMs = optionsView.LingerMs,
                SocketKeepaliveEnable = true, 
                BatchSize = optionsView.BatchSize,
                BatchNumMessages = optionsView.BatchNumMessages,
                SocketReceiveBufferBytes = optionsView.SocketReceiveBufferBytes,
                SocketSendBufferBytes = optionsView.SocketSendBufferBytes
            };
            
            return new ProducerBuilder<string, string>(producerConfig).Build();
        }
        
        IProducer<string, string> singleton = null!;

        if (optionsView.SingletonProducer)
        {
            singleton = CreateProducer();
        }
        
        _spammerStrategyFactory = async _ =>
        {
            IProducer<string, string> producer; 

            if (optionsView.SingletonProducer)
            {
                producer = singleton;
            }
            else
            {
                producer = CreateProducer();
            }

            var randomJitter = TimeSpan.Zero;
            if (optionsView.StartupJitterMs.HasValue)
            {
                lock (Random.Shared)
                {
                    var jitterMs = Random.Shared.Next(optionsView.StartupJitterMs.Value);
                    randomJitter = TimeSpan.FromMilliseconds(jitterMs);
                }
            }
            
            return new KafkaProducerStrategy(
                producer,
                optionsView.SpammerOptionsView.TestName,
                optionsView.SingletonTopic,
                optionsView.Size,
                randomJitter);
        };
        
        return this;
    }
    
    public SpammerBuilder WithDbContextStrategy(
        IDbContextFactory<AppDbContext> dbContextFactory,
        DbContextStrategyType dbContextStrategyType,
        DataCreationStrategyType dataCreationStrategyType,
        int fixedLengthStringLength)
    {
        async Task HotConnections(CancellationToken cancellationToken)
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            await dbContext.SequentialKeyEntities.Take(1).ToListAsync(cancellationToken);
        }
        
        _spammerStrategyFactory = async (cancellationToken) =>
        {
            await HotConnections(cancellationToken);
            
            return new DbContextSpammerStrategy(new DbContextSpammerStrategyOptions()
            {
                DbContextFactory = dbContextFactory,
                DbContextRetryStrategyType = DbContextRetryStrategyType.FullExpensive,
                OperationStrategy =
                    dbContextStrategyType.CreateDbContextStrategy(
                        dataCreationStrategyType.CreateStrategy(fixedLengthStringLength))
            });
        };
        
        return this;
    }

    public SpammerBuilder WithNginxStrategy(NginxPingMode pingMode)
    {
        if (pingMode == NginxPingMode.SingletonHttpClient)
        {
            var singletonPingClient = serviceProvider.GetRequiredService<NginxPingService>();
            _spammerStrategyFactory = (_) => Task.FromResult((ISpammerStrategy)new NginxStrategy(singletonPingClient));
        }
        else
        {
            _spammerStrategyFactory = (_) => Task.FromResult(
                    (ISpammerStrategy)new NginxStrategy(serviceProvider.GetRequiredService<NginxPingService>()));
        }

        return this;
    }
    
    public SpammerBuilder WithRunnerExecutions(int runnerExecutions)
    {
        _runnerExecutions = runnerExecutions;
        return this;
    }
    
    public SpammerBuilder WithSpammerStrategy(Func<ISpammerStrategy> spammerStrategyFactory)
    {
        _spammerStrategyFactory = (_) => Task.FromResult(spammerStrategyFactory());
        return this;
    }
    
    public SpammerBuilder WithSpammerStrategy(Func<CancellationToken, Task<ISpammerStrategy>> spammerStrategyFactory)
    {
        _spammerStrategyFactory = spammerStrategyFactory;
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

    public SpammerBuilder ApplySpammerOptions(SpammerOptionsView optionsView)
    {
        _parallelRunners = optionsView.ParallelRunners;
        _runnerExecutions = optionsView.RunnerExecutions;
        _testName = optionsView.TestName;
        _spammerParallelEngine = optionsView.SpammerParallelEngineType.CreateEngine();
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
        ArgumentNullException.ThrowIfNull(_spammerStrategyFactory);
        ArgumentNullException.ThrowIfNull(_spammerParallelEngine);

        return Create(new SpammerOptions()
        {
            Logger = logger,
            SpammerStrategyFactory = _spammerStrategyFactory,
            SpammerParallelEngine = _spammerParallelEngine,
            Metrics = new SpammerMetrics(appMetrics, _testName, nameof(Spammer)),
            ParallelRunners = _parallelRunners.Value,
            RunnerExecutions = _runnerExecutions.Value
        });
    }
}