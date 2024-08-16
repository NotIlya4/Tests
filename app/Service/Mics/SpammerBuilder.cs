using Amazon.Runtime;
using Amazon.S3;
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
    IOptions<S3Options> s3Options,
    IServiceProvider serviceProvider)
{
    private int? _runnerExecutions;
    private int? _parallelRunners;
    private string? _testName;
    private SpammerStrategyFactory? _spammerStrategyFactory;
    private ISpammerParallelEngine? _spammerParallelEngine;

    public SpammerBuilder WithS3Strategy(S3StrategyOptionsView s3StrategyOptionsView)
    {
        var s3Client = new AmazonS3Client(
            new BasicAWSCredentials(s3Options.Value.AccessKey, s3Options.Value.SecretKey),
            new AmazonS3Config()
            {
                ServiceURL = "https://storage.yandexcloud.net/"
            });

        _spammerStrategyFactory = async (_, _) => new S3Strategy(s3Client, s3Options.Value.BucketName, KafkaJsonCreator.Create(s3StrategyOptionsView.Size));
        
        return this;
    }
    
    public SpammerBuilder WithKafkaProducerStrategy(KafkaProducerStrategyOptionsView optionsView)
    {
        ProducerConfig CreateProducerConfig(string identity)
        {
            var config = new ProducerConfig()
            {
                BootstrapServers = kafkaOptions.Value.BootstrapServers,
                CompressionType = optionsView.CompressionType,
                EnableIdempotence = optionsView.EnableIdempotence,
                Acks = optionsView.Acks,
                ClientId = $"{optionsView.SpammerOptionsView.TestName}-client-{identity}",
                MessageMaxBytes = optionsView.MaxMsgSize == -1 ? optionsView.Size + 100 : optionsView.MaxMsgSize,
                SocketNagleDisable = optionsView.SocketNagleDisable,
                MaxInFlight = optionsView.MaxInFlight,
                LingerMs = optionsView.LingerMs,
                SocketKeepaliveEnable = true, 
                BatchSize = optionsView.BatchSize,
                BatchNumMessages = optionsView.BatchNumMessages,
                SocketReceiveBufferBytes = optionsView.SocketReceiveBufferBytes,
                SocketSendBufferBytes = optionsView.SocketSendBufferBytes,
                StatisticsIntervalMs = 1000,
                Partitioner = optionsView.Partitioner
            };
            if (optionsView.DebugLogs)
                config.Debug = "broker,topic,msg";

            return config;
        }

        var producerContainer = new KafkaProducerContainer(CreateProducerConfig, optionsView.SingletonProducer);
        
        _spammerStrategyFactory = async (runnerIndex,_) =>
        {
            return new KafkaProducerStrategy(
                producerContainer.GetProducer(runnerIndex),
                optionsView.SpammerOptionsView.TestName,
                optionsView.TopicPoolSize,
                optionsView.GenerateGuidKey,
                _ => KafkaJsonCreator.Create(optionsView.Size),
                TimeSpan.FromMilliseconds(Random.Shared.Next(optionsView.StartupJitterMs)));
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
        
        _spammerStrategyFactory = async (_,cancellationToken) =>
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
            _spammerStrategyFactory = (_,_) => Task.FromResult((ISpammerStrategy)new NginxStrategy(singletonPingClient));
        }
        else
        {
            _spammerStrategyFactory = (_,_) => Task.FromResult(
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
        _spammerStrategyFactory = (_,_) => Task.FromResult(spammerStrategyFactory());
        return this;
    }
    
    public SpammerBuilder WithSpammerStrategy(SpammerStrategyFactory spammerStrategyFactory)
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
            RunnerExecutions = _runnerExecutions.Value,
            TestName = _testName
        });
    }
}