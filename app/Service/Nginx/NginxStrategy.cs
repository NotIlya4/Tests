using Spam;

namespace Service;

public class NginxStrategy(NginxSpammerStrategyOptions options) : ISpammerStrategy
{
    private readonly object _syncLock = new object();
    private NginxPingService? _singletonPingService;
    
    public Task Prepare(int runnerIndex, Dictionary<object, object> runnerData,
        CancellationToken cancellationToken)
    {
        if (options.NginxPingMode == NginxPingMode.MultipleHttpClients)
        {
            runnerData["ping"] = options.PingServiceFactory.Create();
        }
        else
        {
            lock (_syncLock)
            {
                _singletonPingService ??= options.PingServiceFactory.Create();
            }
        }
        return Task.CompletedTask;
    }

    public async Task Execute(RunnerExecutionContext context, CancellationToken cancellationToken)
    {
        if (options.NginxPingMode == NginxPingMode.MultipleHttpClients)
        {
            var ping = (NginxPingService)context.Data["ping"];
            await ping.Ping();
        }
        else
        {
            await _singletonPingService!.Ping();
        }
    }
}