using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using Microsoft.Data.SqlClient;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using PostgresMigrations;
using SqlServerMigrationsBuilder;

namespace Service;

public static class AppExtensions
{
    public static IServiceCollection AddConfiguredOpenTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(x =>
            {
                x.AddService("sql-server");
            })
            .WithMetrics(x =>
            {
                x.AddRuntimeInstrumentation();
                x.AddProcessInstrumentation();
                x.AddMeter(AppMetrics.MeterName);
                x.AddOtlpExporter((_, readerOptions) =>
                    readerOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 5000);
            });
        return services;
    }

    public static IServiceCollection AddDependencyBox(this IServiceCollection services)
    {
        services.AddTransient<SqlServerDependencyBox>(x => 
            new SqlServerDependencyBox(new SqlServerDependencyBoxOptions 
            { 
                DefaultConn = x.GetRequiredService<ConnectionStringRepository>().GetSqlServerConn(),
                MigrationAssembly = typeof(SqlServerMigrationsBuilderAnchor).Assembly.GetName().Name!
            }));

        services.AddTransient<PostgresDependencyBox>(x =>
            new PostgresDependencyBox(new PostgresDependencyBoxOptions()
            {
                DefaultConn = x.GetRequiredService<ConnectionStringRepository>().GetPostgresConnBuilder().ConnectionString,
                MigrationAssembly = typeof(PostgresMigrationsAnchor).Assembly.GetName().Name!
            }));

        return services;
    }

    public static IServiceCollection AddAspNetCoreStaff(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddHealthChecks();
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });;

        return services;
    }

    public static IServiceCollection AddMics(this IServiceCollection services)
    {
        services.AddSingleton<AppMetrics>();
        services.AddSingleton<ConnectionStringRepository>();

        return services;
    }

    public static IServiceCollection AddNginxStrategyServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpClient<NginxPingService>();
        services.AddTransient<NginxPingService>(x =>
        {
            var client = x.GetRequiredService<HttpClient>();
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue()
            {
                NoCache = true
            };
            return new NginxPingService(client, config.GetRequiredSection("NginxAddress").Value!);
        });
        services.AddTransient<NginxPingServiceFactory>();

        return services;
    }
    
    public static string GetSqlServerConn(this IConfiguration config, string key)
    {
        return config.GetSqlServerConnBuilder(key).ConnectionString;
    }
    
    public static SqlConnectionStringBuilder GetSqlServerConnBuilder(this IConfiguration config, string key)
    {
        var section = config.GetSection(key);
        var builder = new SqlConnectionStringBuilder();
        
        if (section.Value is null)
        {
            var dictConfig = section.GetChildren().ToDictionary(k => k.Key, v => v.Value);
            foreach (KeyValuePair<string,string?> parameter in dictConfig)
            {
                builder.Add(parameter.Key, parameter.Value ?? "");
            }
        }
        else
        {
            builder.ConnectionString = section.Value;
        }

        return builder;
    }
    
    public static NpgsqlConnectionStringBuilder GetPostgresConnBuilder(this IConfiguration config, string key)
    {
        var section = config.GetSection(key);
        var builder = new NpgsqlConnectionStringBuilder();
        
        if (section.Value is null)
        {
            var dictConfig = section.GetChildren().ToDictionary(k => k.Key, v => v.Value);
            foreach (KeyValuePair<string,string?> parameter in dictConfig)
            {
                builder.Add(parameter.Key, parameter.Value ?? "");
            }
        }
        else
        {
            builder.ConnectionString = section.Value;
        }

        return builder;
    }
}