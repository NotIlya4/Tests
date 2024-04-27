namespace Service;

public class PostgresDependencyBoxOptions
{
    public string? DefaultConn { get; set; }
    public required string MigrationAssembly { get; set; }
}