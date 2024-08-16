using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Spam;

public class Spammer(SpammerOptions options)
{
    private readonly ILogger<Spammer>? _logger = options.Logger;
    private readonly ISpammerMetrics? _metrics = options.Metrics;
    
    public async Task<SpammerRunResult> Run(CancellationToken cancellationToken)
    {
        var preparationStartTime = Stopwatch.GetTimestamp();

        var spammers = new ConcurrentDictionary<int, ISpammerStrategy>();

        await options.SpammerParallelEngine.ParallelRun(
            options.ParallelRunners,
            async i =>
            {
                spammers[i] = await options.SpammerStrategyFactory(i, cancellationToken);
            },
            cancellationToken);

        var preparationTime = Stopwatch.GetElapsedTime(preparationStartTime);
        _logger?.LogInformation("Test \"{TestName}\", preparations finished in {PreparationTime}", options.TestName, preparationTime);

        var runStartTime = Stopwatch.GetTimestamp();

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
                            _logger?.LogError(e, "Test \"{TestName}\", exception during RunCore occured in {i} runner", options.TestName, i);
                        }
                    },
                    options.RunnerExecutions);
            },
            cancellationToken);
        
        var runElapsed = Stopwatch.GetElapsedTime(runStartTime);
        _logger?.LogInformation("Test \"{TestName}\", run finished in {RunElapsed}", options.TestName, runElapsed);

        return new SpammerRunResult
        {
            PreparationDuration = preparationTime,
            RunDuration = runElapsed
        };
    }

    private async Task DecorateWithMetrics(Func<RunnerExecutionContext, Task> action, RunnerExecutionContext context)
    {
        var startTime = Stopwatch.GetTimestamp();

        await action(context);

        var elapsed = Stopwatch.GetElapsedTime(startTime);
        _metrics?.RecordExecutionProcessed(elapsed, context);
        _logger?.LogDebug("Test \"{TestName}\", runner {RunnerIndex}, run {Run}, elapsed {Elapsed}", options.TestName, context.RunnerIndex,
            context.CurrentExecution, elapsed);
    }
}