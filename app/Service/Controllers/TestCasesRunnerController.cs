using Microsoft.AspNetCore.Mvc;
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
    public async Task<SpammerResultView> TestSqlServer(SqlServerStrategyView view, CancellationToken cancellationToken)
    {
        var connBuilder = _connectionStringRepository.GetSqlServerConnBuilder();

        connBuilder.InitialCatalog = view.TestName;
        if (view.Server is not null)
            connBuilder.DataSource = view.Server;
        
        _sqlServerDependencyBox.SetConn(connBuilder.ConnectionString);

        await _sqlServerDependencyBox.Migrate();

        var spammer = _spammerBuilder
            .ApplyView(view)
            .WithDbContextStrategy(
                _sqlServerDependencyBox.DbContextFactory,
                view.DbContextStrategyType,
                view.DataCreationStrategyType,
                view.FixedLengthStringLength)
            .Build();

        var result = await spammer.Run(cancellationToken);

        return SpammerResultView.FromModel(result);
    }
    
    [HttpPost("tests/postgres")]
    public async Task<SpammerResultView> TestPostgres(PostgresStrategyViewOptions view, CancellationToken cancellationToken)
    {
        var connBuilder = _connectionStringRepository.GetPostgresConnBuilder();
        connBuilder.Database = view.TestName;
        _postgresDependencyBox.SetConn(connBuilder.ConnectionString);

        await _postgresDependencyBox.Migrate();

        var builder = _spammerBuilder
            .ApplyView(view)
            .WithDbContextStrategy(
                _postgresDependencyBox.DbContextFactory,
                view.DbContextStrategyType,
                view.DataCreationStrategyType,
                view.FixedLengthStringLength);

        if (view.Dapper)
        {
            _spammerBuilder.WithSpammerStrategy(new PostgresDapperGuidStrategy(new PostgresDapperGuidStrategyOptions()
            {
                Conn = _postgresDependencyBox.Conn,
                DataCreationStrategy = new GuidDataCreationStrategy()
            }));
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
}