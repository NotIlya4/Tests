using Spammer;

namespace Service;

public interface ISqlServerEntityCreationStrategy
{
    Task Create(RunnerExecutionContext context);
}