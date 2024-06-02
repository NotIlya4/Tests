using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Service.Enums;
using Spam;

namespace Service;

public class TestsController
{
    private readonly ConnectionStringRepository _connectionStringRepository;
    private readonly SqlServerDependencyBox _sqlServerDependencyBox;
    private readonly PostgresDependencyBox _postgresDependencyBox;
    private readonly SpammerBuilder _spammerBuilder;

    public TestsController(
        ConnectionStringRepository connectionStringRepository,
        SqlServerDependencyBox sqlServerDependencyBox,
        PostgresDependencyBox postgresDependencyBox,
        SpammerBuilder spammerBuilder)
    {
        _connectionStringRepository = connectionStringRepository;
        _sqlServerDependencyBox = sqlServerDependencyBox;
        _postgresDependencyBox = postgresDependencyBox;
        _spammerBuilder = spammerBuilder;
    }
    
    [HttpPost("tests/sql-server")]
    public async Task<SpammerResultView> TestSqlServer(SqlServerStrategyOptionsView optionsView, CancellationToken cancellationToken)
    {
        var connBuilder = _connectionStringRepository.GetSqlServerConnBuilder();

        connBuilder.InitialCatalog = optionsView.TestName;
        if (optionsView.Server is not null)
            connBuilder.DataSource = optionsView.Server;
        
        _sqlServerDependencyBox.SetConn(connBuilder.ConnectionString);

        await _sqlServerDependencyBox.Migrate();

        var spammer = _spammerBuilder
            .ApplyView(optionsView)
            .WithDbContextStrategy(
                _sqlServerDependencyBox.DbContextFactory,
                optionsView.DbContextStrategyType,
                optionsView.DataCreationStrategyOptions.DataCreationStrategyType,
                optionsView.DataCreationStrategyOptions.FixedLengthStringLength)
            .Build();

        var result = await spammer.Run(cancellationToken);

        return SpammerResultView.FromModel(result);
    }
    
    [HttpPost("tests/postgres")]
    public async Task<SpammerResultView> TestPostgres(PostgresStrategyOptionsView optionsView, CancellationToken cancellationToken)
    {
        var connBuilder = _connectionStringRepository.GetPostgresConnBuilder();
        connBuilder.Database = optionsView.TestName;
        _postgresDependencyBox.SetConn(connBuilder.ConnectionString);

        await _postgresDependencyBox.Migrate();

        var builder = _spammerBuilder
            .ApplyView(optionsView)
            .WithDbContextStrategy(
                _postgresDependencyBox.DbContextFactory,
                optionsView.DbContextStrategyType,
                optionsView.DataCreationStrategyOptions.DataCreationStrategyType,
                optionsView.DataCreationStrategyOptions.FixedLengthStringLength);

        if (!optionsView.IsDbContextStrategy)
        {
            _spammerBuilder.WithSpammerStrategy(optionsView.PostgresStrategyType.CreateStrategy(
                _postgresDependencyBox.Conn,
                optionsView.DataCreationStrategyOptions.CreateStrategy(),
                optionsView.SelectStrategyType,
                optionsView.Limit));
        }
        
        var spammer = builder.Build();

        var result = await spammer.Run(cancellationToken);

        return SpammerResultView.FromModel(result);
    }
    
    [HttpPost("tests/nginx")]
    public async Task<SpammerResultView> TestNginx(NginxStrategyView view, CancellationToken cancellationToken)
    {
        var spammer = _spammerBuilder
            .ApplyView(view)
            .WithNginxStrategy(view.PingMode)
            .Build();

        var result = await spammer.Run(cancellationToken);

        return SpammerResultView.FromModel(result);
    }
    
    [HttpPost("tests/sanity")]
    public async Task<SpammerResultView> TestSanity([DefaultValue(1000)] int sleepMs, CancellationToken cancellationToken)
    {
        var times = 60000 / sleepMs;
        
        var spammer = _spammerBuilder
            .WithParallelRunners(1)
            .WithRunnerExecutions(times)
            .WithSpammerStrategy(new SanityStrategy(sleepMs))
            .WithTestName("sanity")
            .WithParallelEngine(new ForParallelEngine())
            .Build();

        var result = await spammer.Run(cancellationToken);

        return SpammerResultView.FromModel(result);
    }
}