using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Spammer;

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
                x.AddMeter(AppMetrics.MeterName);
                x.AddOtlpExporter((_, readerOptions) =>
                    readerOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 5000);
            });
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