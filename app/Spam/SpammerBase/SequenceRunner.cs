namespace Spam;

public static class SequenceRunner
{
    public static async Task Run(Func<int, Task> func, int runCount)
    {
        for (int i = 0; i < runCount; i++)
        {
            await func(i);
        }
    }
}