using Spammer;

namespace Service;

public class NginxSpammerBuilder(NginxSpammerDependencies spammerDependencies, SpammerBuilderBaseDependencies dependencies) : SpammerBuilderBase<NginxSpammerBuilder, NginxSpammer>(dependencies)
{
    private NginxPingMode? _pingMode;

    public NginxSpammerBuilder WithPingMode(NginxPingMode pingMode)
    {
        _pingMode = pingMode;
        return this;
    }
    
    protected override NginxSpammer Create(SpammerOptions spammerOptions)
    {
        ArgumentNullException.ThrowIfNull(_pingMode);
        
        return new NginxSpammer(spammerDependencies, spammerOptions, _pingMode.Value);
    }
}