using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Spam;

public class Spammer(SpammerOptions options)
{
    private readonly int _parallelRunners = options.ParallelRunners;
    private readonly int _runnerExecutions = options.RunnerExecutions;
    private readonly ILogger<Spammer>? _logger = options.Logger;
    private readonly ISpammerMetrics? _metrics = options.Metrics;
    
    public async Task<SpammerRunResult> Run(CancellationToken cancellationToken)
    {
        var datas = new ConcurrentDictionary<int, Dictionary<object, object>>(_parallelRunners, _parallelRunners);
        
        var preparationStopwatch = Stopwatch.StartNew();

        await options.SpammerParallelEngine.ParallelRun(
            options.ParallelRunners,
            async i =>
            {
                var data = new Dictionary<object, object>();
                datas[i] = data;

                await options.SpammerStrategy.Prepare(i, data, cancellationToken);
            },
            cancellationToken);
        
        preparationStopwatch.Stop();
        var preparationTime = preparationStopwatch.Elapsed;
        _logger?.LogInformation("Preparation hooks finished in {PreparationTime}", preparationTime);

        var runStopwatch = Stopwatch.StartNew();

        await options.SpammerParallelEngine.ParallelRun(
            options.ParallelRunners,
            async i =>
            {
                var data = datas[i];
                
                await SequenceRunner.Run(
                    async currentRun =>
                    {
                        try
                        {
                            await DecorateWithMetrics(
                                async context => await options.SpammerStrategy.Execute(context, cancellationToken),
                                new RunnerExecutionContext()
                                {
                                    CurrentExecution = currentRun,
                                    Data = data,
                                    RunnerIndex = i
                                });
                        }
                        catch (Exception e)
                        {
                            _logger?.LogError(e, "Exception during RunCore occured in {i} runner", i);
                        }
                    },
                    _runnerExecutions);
            },
            cancellationToken);
        
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
}