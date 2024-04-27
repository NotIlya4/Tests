namespace Spam;

public class FixedLengthStringDataCreationStrategy(int length) : ISimpleDataCreationStrategy<string>
{
    public string CreateData()
    {
        return string.Join("", Enumerable.Repeat(0, length).Select(n => (char)new Random().Next(33, 126)));
    }
}