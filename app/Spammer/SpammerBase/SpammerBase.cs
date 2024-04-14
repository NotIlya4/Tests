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
    
    public async Task<SpammerRunResult> Run()
    {
        var datas = new ConcurrentDictionary<int, Dictionary<object, object>>(_parallelRunners, _parallelRunners);
        
        var preparationStopwatch = Stopwatch.StartNew();

        await Parallel.ForAsync(0, _parallelRunners, new ParallelOptions(){MaxDegreeOfParallelism = _parallelRunners},
            async (i, _) =>
            {
                var data = new Dictionary<object, object>();
                datas[i] = data;

                await OnRunnerCreating(i, data);
            });
        
        preparationStopwatch.Stop();
        var preparationTime = preparationStopwatch.Elapsed;
        _logger?.LogInformation("Preparation hooks finished in {PreparationTime}", preparationTime);

        var runStopwatch = Stopwatch.StartNew();
        
        await Parallel.ForAsync(0, _parallelRunners, new ParallelOptions(){MaxDegreeOfParallelism = _parallelRunners},
            async (i, _) =>
            {
                await SequenceRunner.Run(
                    async currentRun =>
                    {
                        try
                        {
                            await DecorateWithMetrics(
                                ExecuteAsync,
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
        SpinWaiter.SpinForBegin(TimeSpan.FromMilliseconds(5));

        var stopwatch = Stopwatch.StartNew();

        await action(context);
        
        stopwatch.Stop();
        _metrics?.RecordExecutionProcessed(stopwatch.Elapsed, context);
        
        SpinWaiter.SpinForEnd(TimeSpan.FromMilliseconds(5));
    }
    
    protected virtual Task OnRunnerCreating(
        int runnerIndex,
        Dictionary<object, object> runnerData)
    {
        return Task.CompletedTask;
    }

    protected abstract Task ExecuteAsync(RunnerExecutionContext context);
}