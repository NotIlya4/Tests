namespace Spam;

public class StringEntityInsertStrategyOptions
{
    public required ISimpleDataCreationStrategy<string> DataCreationStrategy { get; set; }
}