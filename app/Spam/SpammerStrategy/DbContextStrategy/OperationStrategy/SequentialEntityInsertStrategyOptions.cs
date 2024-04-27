namespace Spam;

public class SequentialEntityInsertStrategyOptions
{
    public required ISimpleDataCreationStrategy<string> DataCreationStrategy { get; set; }
}