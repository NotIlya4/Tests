using Spammer;

namespace Service;

public class NginxSpammer(
    NginxSpammerDependencies dependencies,
    SpammerOptions spammerOptions,
    NginxPingMode pingMode) : SpammerBase(spammerOptions) 
{
    protected override Task OnRunnerCreating(int runnerIndex, Dictionary<object, object> runnerData)
    {
        if (pingMode == NginxPingMode.MultipleHttpClients)
        {
            runnerData["ping"] = dependencies.ServiceProvider.GetRequiredService<NginxPing>();
        }
        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(RunnerExecutionContext context)
    {
        var ping = dependencies.Ping;
        if (pingMode == NginxPingMode.MultipleHttpClients)
        {
            ping = (NginxPing)context.Data["ping"];
        }
        await ping.Ping();
    }
}