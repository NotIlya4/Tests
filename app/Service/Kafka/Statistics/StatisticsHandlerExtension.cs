using Confluent.Kafka;
using Newtonsoft.Json;
using Service.Model;

namespace Service;

/// <summary>
/// 
/// </summary>
public static class StatisticsHandlerExtension
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ProducerBuilder<K"></param>
    /// <param name="builder"></param>
    /// <param name="handler"></param>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <returns></returns>
    public static ProducerBuilder<K, V> HandleStatistics<K, V>(this ProducerBuilder<K, V> builder, IStatisticsHandler handler)
    {
        builder.SetStatisticsHandler((_, json) =>
        {
            var statistics = JsonConvert.DeserializeObject<Statistics>(json);
            handler.Publish(statistics);
        });
        return builder;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ConsumerBuilder<K"></param>
    /// <param name="builder"></param>
    /// <param name="handler"></param>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <returns></returns>
    public static void HandleStatistics<K, V>(this ConsumerBuilder<K, V> builder, IStatisticsHandler handler)
    {
        builder.SetStatisticsHandler((_, json) =>
        {
            var statistics = JsonConvert.DeserializeObject<Statistics>(json);
            handler.Publish(statistics);
        });
    }
}