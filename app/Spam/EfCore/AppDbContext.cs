using Microsoft.EntityFrameworkCore;

namespace Spam;

public class AppDbContext : DbContext
{
    private readonly AppDbContextConfigurator _appDbContextConfigurator;
    public DbSet<SequentialEntity> SequentialEntities { get; set; }
    public DbSet<StringEntity> StringEntities { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options, AppDbContextConfigurator appDbContextConfigurator) : base(options)
    {
        _appDbContextConfigurator = appDbContextConfigurator;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        _appDbContextConfigurator.ApplyToDbContextOptionsBuilder(optionsBuilder);
    }
}