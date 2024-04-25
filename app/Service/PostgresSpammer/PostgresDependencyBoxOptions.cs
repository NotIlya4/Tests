namespace Service.PostgresSpammer;

public class PostgresDependencyBoxOptions
{
    public string? DefaultConn { get; set; }
    public required string MigrationAssembly { get; set; }
}