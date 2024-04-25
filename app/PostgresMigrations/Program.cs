using Microsoft.EntityFrameworkCore;
using PostgresMigrations;
using Spammer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(x =>
    x.UseNpgsql(x => x.MigrationsAssembly(typeof(PostgresMigrationsAnchor).Assembly.GetName().Name)));
builder.Services.AddTransient<AppDbContextConfigurator>();

var app = builder.Build();

app.Run();