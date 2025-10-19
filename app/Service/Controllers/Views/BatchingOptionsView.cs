using System.ComponentModel;

namespace Service;

public class BatchingOptionsView
{
    public SpammerOptionsView SpammerOptionsView { get; set; } = new SpammerOptionsView();

    [DefaultValue(true)]
    public bool IsTaskDelay { get; set; } = true;
    
    [DefaultValue(false)]
    public bool UseYieldInsteadOfDelay { get; set; } = false;

    [DefaultValue(15)]
    public int SleepMs { get; set; } = 15;
    
    [DefaultValue(100)]
    public int LongSleepMs { get; set; } = 100;
    
    [DefaultValue(5)]
    public double LongSleepChance { get; set; } = 5;
    
    [DefaultValue(1)]
    public int SpinLoops { get; set; } = 1;
    
    [DefaultValue(1)]
    public int InnerSpinLoops { get; set; } = 1;

    [DefaultValue(20)]
    public int MaxBatchSize { get; set; } = 20;
    
    [DefaultValue(20)]
    public int LingerMs { get; set; } = 20;
}