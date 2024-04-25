using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using PostgresMigrations;
using Service;
using Service.PostgresSpammer;
using SqlServerMigrationsBuilder;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddHealthChecks();
services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });;
services.AddSingleton<AppMetrics>();
services.AddSingleton<ConnectionStringRepository>();
services.AddHttpClient<NginxPing>();
services.AddTransient<NginxPing>(x =>
{
    var client = x.GetRequiredService<HttpClient>();
    client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue()
    {
        NoCache = true
    };
    return new NginxPing(client, config.GetRequiredSection("NginxAddress").Value!);
});
services.AddTransient<SpammerBuilderBaseDependencies>();
services.AddTransient<NginxSpammerDependencies>();
services.AddTransient<NginxSpammerBuilderFactory>();
services.AddTransient<SqlServerDependencyBox>(x => 
    new SqlServerDependencyBox(
        new SqlServerDependencyBoxOptions 
        { 
            DefaultConn = x.GetRequiredService<ConnectionStringRepository>().GetSqlServerConn(),
            MigrationAssembly = typeof(SqlServerMigrationsBuilderAnchor).Assembly.GetName().Name!
        }));
services.AddTransient<IDbContextSpammerStrategy, EntitySpammerStrategy>();
services.AddTransient<IDbContextSpammerStrategy, GuidEntitySpammerStrategy>();
services.AddTransient<PostgresDependencyBox>(x =>
    new PostgresDependencyBox(new PostgresDependencyBoxOptions()
    {
        DefaultConn = x.GetRequiredService<ConnectionStringRepository>().GetPostgresConnBuilder().ConnectionString,
        MigrationAssembly = typeof(PostgresMigrationsAnchor).Assembly.GetName().Name!
    }));
services.AddTransient<SmartDbContextSpammerBuilderFactory>();

services.AddConfiguredOpenTelemetry();

var app = builder.Build();

app.UseHealthChecks("/healthz");
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();