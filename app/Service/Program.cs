using Service;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddOptions();
services.AddOptions<KafkaOptions>().BindConfiguration("Kafka");
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