using Spammer;

namespace Service;

public abstract class SpammerBuilderBase<TSpammerBuilderBase, TSpammer>(SpammerBuilderBaseDependencies dependencies)
    where TSpammerBuilderBase : SpammerBuilderBase<TSpammerBuilderBase, TSpammer>
    where TSpammer : SpammerBase
{
    private int? _runnerExecutions;
    private int? _parallelRunners;
    private string? _testName;
    
    public TSpammerBuilderBase WithRunnerExecutions(int runnerExecutions)
    {
        _runnerExecutions = runnerExecutions;
        return (TSpammerBuilderBase)this;
    }

    public TSpammerBuilderBase WithParallelRunners(int parallelRunners)
    {
        _parallelRunners = parallelRunners;
        return (TSpammerBuilderBase)this;
    }
    
    public TSpammerBuilderBase WithTestName(string testName)
    {
        _testName = testName;
        return (TSpammerBuilderBase)this;
    }

    public virtual TSpammerBuilderBase ApplyView(SpammerBaseView baseView)
    {
        _parallelRunners = baseView.ParallelRunners;
        _runnerExecutions = baseView.RunnerExecutions;
        _testName = baseView.TestName;
        return (TSpammerBuilderBase)this;
    }

    protected abstract TSpammer Create(SpammerOptions spammerOptions);

    public TSpammer Build()
    {
        ArgumentNullException.ThrowIfNull(_parallelRunners);
        ArgumentNullException.ThrowIfNull(_runnerExecutions);
        ArgumentNullException.ThrowIfNull(_testName);

        return Create(new SpammerOptions()
        {
            Logger = dependencies.Logger,
            Metrics = new SpammerMetrics(dependencies.Metrics, _testName, typeof(TSpammer).Name),
            ParallelRunners = _parallelRunners.Value,
            RunnerExecutions = _runnerExecutions.Value
        });
    }
}