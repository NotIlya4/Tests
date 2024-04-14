using Microsoft.EntityFrameworkCore;

namespace Spammer;

public class AppDbContext : DbContext
{
    private readonly AppDbContextConfigurator _appDbContextConfigurator;
    public DbSet<Entity> Entities { get; set; }
    public DbSet<GuidEntity> GuidEntities { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options, AppDbContextConfigurator appDbContextConfigurator) : base(options)
    {
        _appDbContextConfigurator = appDbContextConfigurator;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        _appDbContextConfigurator.Configure(optionsBuilder);
    }
}