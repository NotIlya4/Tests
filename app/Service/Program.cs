using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using PostgresMigrations;
using Service;
using SqlServerMigrationsBuilder;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

ThreadPool.SetMaxThreads(
    builder.Configuration.GetValue<int>("WorkerThreads"),
    builder.Configuration.GetValue<int>("CWorkerThreads"));

services.AddAspNetCoreStaff();
services.AddDependencyBox();
services.AddMics();
services.AddNginxStrategyServices(builder.Configuration);
services.AddConfiguredOpenTelemetry();
services.AddTransient<SpammerBuilder>();

var app = builder.Build();

app.UseHealthChecks("/healthz");
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();