namespace Service;

public class SqlServerDependencyBoxOptions
{
    public string? DefaultConn { get; set; }
    public required string MigrationAssembly { get; set; }
}