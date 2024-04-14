using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using Service;
using Spammer;

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
services.AddTransient<SqlServerSpammerBuilderFactory>();
services.AddTransient<SqlServerSpammerDependencies>();
services.AddSingleton<SqlServerEntityCreationStrategyProvider>();
services.AddScoped<AppDbContextConfigurator>();
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

services.AddConfiguredDbContextFactory(config.GetSqlServerConn("SqlServerConn"));
services.AddConfiguredOpenTelemetry();

var app = builder.Build();

app.UseHealthChecks("/healthz");
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();