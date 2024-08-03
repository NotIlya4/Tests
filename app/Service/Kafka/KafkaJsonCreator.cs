using System.Text;

namespace Service;

public class KafkaJsonCreator
{
    public static string Create(int size)
    {
        var message = new StringBuilder("{");
        var i = 0;

        while (message.Length < size)
        {
            i++;
            message.Append($"\"property{i}\": \"value{i}\",");
        }
        return message.Append("}").ToString();
    }
}