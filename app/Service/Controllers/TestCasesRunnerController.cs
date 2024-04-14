using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spammer;

namespace Service;

public class TestsController
{
    private readonly SqlServerSpammerBuilderFactory _sqlServerSpammerBuilderFactory;
    private readonly NginxSpammerBuilderFactory _nginxSpammerBuilderFactory;
    private readonly AppDbContextConfigurator _dbContextConfigurator;
    private readonly ConnectionStringRepository _connectionStringRepository;
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    public TestsController(
        SqlServerSpammerBuilderFactory sqlServerSpammerBuilderFactory,
        NginxSpammerBuilderFactory nginxSpammerBuilderFactory,
        AppDbContextConfigurator dbContextConfigurator,
        ConnectionStringRepository connectionStringRepository,
        IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _sqlServerSpammerBuilderFactory = sqlServerSpammerBuilderFactory;
        _nginxSpammerBuilderFactory = nginxSpammerBuilderFactory;
        _dbContextConfigurator = dbContextConfigurator;
        _connectionStringRepository = connectionStringRepository;
        _dbContextFactory = dbContextFactory;
    }
    
    [HttpPost("tests/sql-server")]
    public async Task<SpammerResultView> TestSqlServer(SqlServerSpammerView view)
    {
        var connBuilder = _connectionStringRepository.GetSqlServerConnBuilder();
        connBuilder.InitialCatalog = view.TestName;
        var conn = connBuilder.ConnectionString;
        
        _dbContextConfigurator.Configure(x =>
        {
            x.UseSqlServer(conn);
        });

        await Migrate();
        
        var builder = _sqlServerSpammerBuilderFactory.Create();

        var spammer = builder
            .ApplyView(view)
            .WithEntityType(view.EntityType)
            .Build();

        var result = await spammer.Run();

        return SpammerResultView.FromModel(result);
    }
    
    [HttpPost("tests/nginx")]
    public async Task<SpammerResultView> TestNginx(NginxSpammerView view)
    {
        var spammer = _nginxSpammerBuilderFactory.Create()
            .ApplyView(view)
            .WithPingMode(view.PingMode)
            .Build();

        var result = await spammer.Run();

        return SpammerResultView.FromModel(result);
    }

    private async Task Migrate()
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.Database.MigrateAsync();
    }
}