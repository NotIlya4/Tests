using System.Text;
using IronSnappy;
using Prometheus;
using Service;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddOptions();
services.AddOptions<KafkaOptions>().BindConfiguration("Kafka");
services.AddAspNetCoreStaff();
services.AddDependencyBox();
services.AddMics();
services.AddNginxStrategyServices(builder.Configuration);
services.AddTransient<SpammerBuilder>();
services.AddMetrics();

var app = builder.Build();

app.MapMetrics();
app.UseHealthChecks("/healthz");
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();