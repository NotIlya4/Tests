﻿using Microsoft.EntityFrameworkCore;
using Spammer;

namespace Service.PostgresSpammer;

public class PostgresDependencyBox
{
    private readonly IServiceCollection _serviceCollection = new ServiceCollection();

    public IServiceProvider Services { get; }

    public IDbContextFactory<AppDbContext> DbContextFactory =>
        Services.GetRequiredService<IDbContextFactory<AppDbContext>>();
    
    public PostgresDependencyBox(PostgresDependencyBoxOptions options)
    {
        _serviceCollection.AddDbContextFactory<AppDbContext>(
            x => x.UseNpgsql(
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
        ConfigureDbContext(x => x.UseNpgsql(conn));
    }

    public void ConfigureDbContext(Action<DbContextOptionsBuilder> action)
    {
        var configurator = Services.GetRequiredService<AppDbContextConfigurator>();
        configurator.Configure(action);
    }
}