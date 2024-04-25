using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Spammer;

public abstract class SpammerBase(SpammerOptions options)
{
    private readonly int _parallelRunners = options.ParallelRunners;
    private readonly int _runnerExecutions = options.RunnerExecutions;
    private readonly ILogger<SpammerBase>? _logger = options.Logger;
    private readonly ISpammerMetrics? _metrics = options.Metrics;
    
    public async Task<SpammerRunResult> Run(CancellationToken cancellationToken)
    {
        var datas = new ConcurrentDictionary<int, Dictionary<object, object>>(_parallelRunners, _parallelRunners);
        
        var preparationStopwatch = Stopwatch.StartNew();

        await Parallel.ForAsync(0, _parallelRunners, new ParallelOptions(){MaxDegreeOfParallelism = _parallelRunners, CancellationToken = cancellationToken},
            async (i, _) =>
            {
                var data = new Dictionary<object, object>();
                datas[i] = data;

                await OnRunnerCreating(i, data, cancellationToken);
            });
        
        preparationStopwatch.Stop();
        var preparationTime = preparationStopwatch.Elapsed;
        _logger?.LogInformation("Preparation hooks finished in {PreparationTime}", preparationTime);

        var runStopwatch = Stopwatch.StartNew();
        
        await Parallel.ForAsync(0, _parallelRunners, new ParallelOptions(){MaxDegreeOfParallelism = _parallelRunners, CancellationToken = cancellationToken},
            async (i, _) =>
            {
                await SequenceRunner.Run(
                    async currentRun =>
                    {
                        try
                        {
                            await DecorateWithMetrics(
                                context => Execute(context, cancellationToken),
                                new RunnerExecutionContext() 
                                { 
                                    CurrentExecution = currentRun,
                                    Data = datas[i],
                                    RunnerIndex = i
                                });
                        }
                        catch (Exception e)
                        {
                            _logger?.LogError(e, "Exception during RunCore occured in {i} runner", i);
                        }
                    },
                    _runnerExecutions);
            });
        
        runStopwatch.Stop();
        var runElapsed = runStopwatch.Elapsed;
        _logger?.LogInformation("Run finished in {RunElapsed}", runElapsed);

        return new SpammerRunResult
        {
            PreparationDuration = preparationTime,
            RunDuration = runElapsed
        };
    }

    private async Task DecorateWithMetrics(Func<RunnerExecutionContext, Task> action, RunnerExecutionContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        await action(context);
        
        stopwatch.Stop();
        _metrics?.RecordExecutionProcessed(stopwatch.Elapsed, context);
    }
    
    protected virtual Task OnRunnerCreating(
        int runnerIndex,
        Dictionary<object, object> runnerData,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected abstract Task Execute(RunnerExecutionContext context, CancellationToken cancellationToken);
}