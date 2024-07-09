using Spam;

namespace Service;

public class NginxStrategy(NginxPingService nginxPingService) : ISpammerStrategy
{
    public async Task Execute(RunnerExecutionContext context, CancellationToken cancellationToken)
    {
        await nginxPingService.Ping();
    }
}