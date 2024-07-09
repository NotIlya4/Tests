using Microsoft.EntityFrameworkCore;

namespace Spam;

public class AppDbContext : DbContext
{
    private readonly AppDbContextConfigurator _appDbContextConfigurator;
    public DbSet<SequentialEntity> SequentialKeyEntities { get; set; }
    public DbSet<StringEntity> RandomKeyEntities { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options, AppDbContextConfigurator appDbContextConfigurator) : base(options)
    {
        _appDbContextConfigurator = appDbContextConfigurator;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        _appDbContextConfigurator.ApplyToDbContextOptionsBuilder(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<StringEntity>()
            .Property(x => x.Id)
            .HasMaxLength(512);
        
        modelBuilder
            .Entity<SequentialEntity>()
            .Property(x => x.SomeProperty)
            .HasMaxLength(512);
    }
}