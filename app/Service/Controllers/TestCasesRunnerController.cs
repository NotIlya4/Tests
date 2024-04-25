using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.PostgresSpammer;
using Spammer;

namespace Service;

public class TestsController
{
    private readonly SmartDbContextSpammerBuilderFactory _smartDbContextSpammerBuilderFactory;
    private readonly NginxSpammerBuilderFactory _nginxSpammerBuilderFactory;
    private readonly ConnectionStringRepository _connectionStringRepository;
    private readonly SqlServerDependencyBox _sqlServerDependencyBox;
    private readonly PostgresDependencyBox _postgresDependencyBox;

    public TestsController(
        SmartDbContextSpammerBuilderFactory smartDbContextSpammerBuilderFactory,
        NginxSpammerBuilderFactory nginxSpammerBuilderFactory,
        ConnectionStringRepository connectionStringRepository,
        SqlServerDependencyBox sqlServerDependencyBox,
        PostgresDependencyBox postgresDependencyBox)
    {
        _smartDbContextSpammerBuilderFactory = smartDbContextSpammerBuilderFactory;
        _nginxSpammerBuilderFactory = nginxSpammerBuilderFactory;
        _connectionStringRepository = connectionStringRepository;
        _sqlServerDependencyBox = sqlServerDependencyBox;
        _postgresDependencyBox = postgresDependencyBox;
    }
    
    [HttpPost("tests/sql-server")]
    public async Task<SpammerResultView> TestSqlServer(SqlServerSpammerView view, CancellationToken cancellationToken)
    {
        var connBuilder = _connectionStringRepository.GetSqlServerConnBuilder();

        connBuilder.InitialCatalog = view.TestName;
        if (view.Server is not null)
            connBuilder.DataSource = view.Server;
        
        _sqlServerDependencyBox.SetConn(connBuilder.ConnectionString);

        await Migrate();
        
        var builder = _smartDbContextSpammerBuilderFactory.Create(_sqlServerDependencyBox.DbContextFactory);

        var spammer = builder
            .ApplyView(view)
            .WithStrategyType(view.SpammerStrategyType)
            .Build();

        var result = await spammer.Run(cancellationToken);

        return SpammerResultView.FromModel(result);
    }
    
    [HttpPost("tests/postgres")]
    public async Task<SpammerResultView> TestPostgres(SmartDbContextSpammerOptionsView view, CancellationToken cancellationToken)
    {
        var connBuilder = _connectionStringRepository.GetPostgresConnBuilder();

        connBuilder.Database = view.TestName;
        
        _postgresDependencyBox.SetConn(connBuilder.ConnectionString);

        await using var dbContext = await _postgresDependencyBox.DbContextFactory.CreateDbContextAsync();
        await dbContext.Database.MigrateAsync();
        
        var builder = _smartDbContextSpammerBuilderFactory.Create(_postgresDependencyBox.DbContextFactory);

        var spammer = builder
            .ApplyView(view)
            .WithStrategyType(view.SpammerStrategyType)
            .Build();

        var result = await spammer.Run(cancellationToken);

        return SpammerResultView.FromModel(result);
    }
    
    [HttpPost("tests/nginx")]
    public async Task<SpammerResultView> TestNginx(NginxSpammerView view, CancellationToken cancellationToken)
    {
        var spammer = _nginxSpammerBuilderFactory.Create()
            .ApplyView(view)
            .WithPingMode(view.PingMode)
            .Build();

        var result = await spammer.Run(cancellationToken);

        return SpammerResultView.FromModel(result);
    }

    private async Task Migrate()
    {
        await using var dbContext = await _sqlServerDependencyBox.DbContextFactory.CreateDbContextAsync();
        await dbContext.Database.MigrateAsync();
    }
}