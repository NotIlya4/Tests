namespace Spam;

public interface IContentCreationStrategy
{
    string CreateContent(RunnerExecutionContext context);
}