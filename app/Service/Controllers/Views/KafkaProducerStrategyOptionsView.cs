using System.ComponentModel;
using Confluent.Kafka;

namespace Service;

public class KafkaProducerStrategyOptionsView
{
    public SpammerOptionsView SpammerOptionsView { get; set; } = new SpammerOptionsView();

    [DefaultValue(true)]
    public bool SingletonTopic { get; set; } = true;

    [DefaultValue(true)]
    public bool SingletonProducer { get; set; }
    
    [DefaultValue(1024 * 1024)]
    public int Size { get; set; } = 1024 * 1024;
    
    [DefaultValue(-1)]
    public int MaxMsgSize { get; set; } = -1;
    
    [DefaultValue(CompressionType.Snappy)]
    public CompressionType CompressionType { get; set; } = CompressionType.Snappy;
    
    [DefaultValue(Acks.All)]
    public Acks Acks { get; set; } = Acks.All;
    
    [DefaultValue(false)]
    public bool EnableIdempotence { get; set; } = true;

    [DefaultValue(false)]
    public bool SocketNagleDisable { get; set; } = false;

    [DefaultValue(null)]
    public int? MaxInFlight { get; set; } = null;

    public double? LingerMs { get; set; } = null;

    public int? BatchSize { get; set; } = null;

    public int? BatchNumMessages { get; set; } = null;

    public int? StartupJitterMs { get; set; } = null;

    public int? SocketReceiveBufferBytes { get; set; } = null;

    public int? SocketSendBufferBytes { get; set; } = null;
}