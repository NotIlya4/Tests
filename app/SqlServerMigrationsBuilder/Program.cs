using Microsoft.EntityFrameworkCore;
using Spammer;
using SqlServerMigrationsBuilder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<AppDbContextConfigurator>();
builder.Services.AddDbContext<AppDbContext>(x =>
    x.UseSqlServer("", b => b.MigrationsAssembly(typeof(SqlServerMigrationsBuilderAnchor).Assembly.GetName().Name)));

var app = builder.Build();

app.Run();