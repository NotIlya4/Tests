using Spam;

namespace Service;

public class PostgresDapperGuidStrategyOptions
{
    public required string Conn { get; set; }
    public required ISimpleDataCreationStrategy<string> DataCreationStrategy { get; set; }
}