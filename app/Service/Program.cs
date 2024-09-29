using Prometheus;
using Service;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddOptions();
services.AddOptions<KafkaOptions>().BindConfiguration("Kafka");
services.AddOptions<S3Options>().BindConfiguration("S3");
services.AddAspNetCoreStaff();
services.AddDependencyBox();
services.AddMics();
services.AddNginxStrategyServices(builder.Configuration);
services.AddTransient<SpammerBuilder>();
services.AddMetrics();
services.AddBatching();

var app = builder.Build();

app.MapMetrics();
app.UseHealthChecks("/healthz");
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();