namespace Spam;

public interface ISimpleDataCreationStrategy<TDataType>
{
    TDataType CreateData();
}