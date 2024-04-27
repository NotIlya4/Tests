using Microsoft.EntityFrameworkCore;
using Spam;
using SqlServerMigrationsBuilder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<AppDbContextConfigurator>();
builder.Services.AddDbContext<AppDbContext>(x =>
    x.UseSqlServer("", b => b.MigrationsAssembly(typeof(SqlServerMigrationsBuilderAnchor).Assembly.GetName().Name)));

var app = builder.Build();

app.Run();