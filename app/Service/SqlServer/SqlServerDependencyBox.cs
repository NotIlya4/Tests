using Microsoft.EntityFrameworkCore;
using Spammer;

namespace Service;

public class SqlServerDependencyBox
{
    private readonly IServiceCollection _serviceCollection = new ServiceCollection();

    public IServiceProvider Services { get; }

    public IDbContextFactory<AppDbContext> DbContextFactory =>
        Services.GetRequiredService<IDbContextFactory<AppDbContext>>();
    
    public SqlServerDependencyBox(SqlServerDependencyBoxOptions options)
    {
        _serviceCollection.AddDbContextFactory<AppDbContext>(
            x => x.UseSqlServer(
                options.DefaultConn,
                b => b
                    .MigrationsAssembly(options.MigrationAssembly)
                    .EnableRetryOnFailure())
                .EnableDetailedErrors());

        _serviceCollection.AddSingleton<AppDbContextConfigurator>();

        Services = _serviceCollection.BuildServiceProvider();
    }

    public void SetConn(string conn)
    {
        ConfigureDbContext(x => x.UseSqlServer(conn));
    }

    public void ConfigureDbContext(Action<DbContextOptionsBuilder> action)
    {
        var configurator = Services.GetRequiredService<AppDbContextConfigurator>();
        configurator.Configure(action);
    }
}