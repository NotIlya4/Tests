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
        var preparationStopwatch = Stopwatch.StartNew();

        var spammers = new ConcurrentDictionary<int, ISpammerStrategy>();

        await options.SpammerParallelEngine.ParallelRun(
            options.ParallelRunners,
            async i =>
            {
                spammers[i] = await options.SpammerStrategyFactory(cancellationToken);
            },
            cancellationToken);
        
        preparationStopwatch.Stop();
        var preparationTime = preparationStopwatch.Elapsed;
        _logger?.LogInformation("Preparations finished in {PreparationTime}", preparationTime);

        var runStopwatch = Stopwatch.StartNew();

        await options.SpammerParallelEngine.ParallelRun(
            options.ParallelRunners,
            async i =>
            {
                var spammer = spammers[i];
                await SequenceRunner.Run(
                    async currentRun =>
                    {
                        try
                        {
                            await DecorateWithMetrics(
                                async context => await spammer.Execute(context, cancellationToken),
                                new RunnerExecutionContext()
                                {
                                    CurrentExecution = currentRun,
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