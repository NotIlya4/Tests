using System.ComponentModel;

namespace Service;

public class S3StrategyOptionsView
{
    public SpammerOptionsView SpammerOptionsView { get; set; } = new SpammerOptionsView();

    [DefaultValue(1_000_000)]
    public int Size { get; set; } = 1_000_000;
}