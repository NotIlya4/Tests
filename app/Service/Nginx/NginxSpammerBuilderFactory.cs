namespace Service;

public class NginxSpammerBuilderFactory(
    NginxSpammerDependencies spammerDependencies,
    SpammerBuilderBaseDependencies dependencies)
{
    public NginxSpammerBuilder Create()
    {
        return new NginxSpammerBuilder(spammerDependencies, dependencies);
    }
}