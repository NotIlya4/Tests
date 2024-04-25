using Spammer;

namespace Service;

public class NginxSpammer(
    NginxSpammerDependencies dependencies,
    SpammerOptions spammerOptions,
    NginxPingMode pingMode) : SpammerBase(spammerOptions) 
{
    protected override Task OnRunnerCreating(int runnerIndex, Dictionary<object, object> runnerData,
        CancellationToken cancellationToken)
    {
        if (pingMode == NginxPingMode.MultipleHttpClients)
        {
            runnerData["ping"] = dependencies.ServiceProvider.GetRequiredService<NginxPing>();
        }
        return Task.CompletedTask;
    }

    protected override async Task Execute(RunnerExecutionContext context, CancellationToken cancellationToken)
    {
        var ping = dependencies.Ping;
        if (pingMode == NginxPingMode.MultipleHttpClients)
        {
            ping = (NginxPing)context.Data["ping"];
        }
        await ping.Ping();
    }
}