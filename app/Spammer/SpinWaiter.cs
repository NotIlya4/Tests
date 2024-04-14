using System.Diagnostics;

namespace Spammer;

public class SpinWaiter
{
    public static void SpinForBegin(TimeSpan timeSpan)
    {
        var sw = Stopwatch.StartNew();

        while (sw.Elapsed < timeSpan)
        {
            Math.Sqrt(4);
        }
    }
    
    public static void SpinForEnd(TimeSpan timeSpan)
    {
        var sw = Stopwatch.StartNew();

        while (sw.Elapsed < timeSpan)
        {
            Math.Sqrt(4);
        }
    }
}