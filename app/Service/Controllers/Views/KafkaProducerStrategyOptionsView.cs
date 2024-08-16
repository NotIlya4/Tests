using System.ComponentModel;
using Confluent.Kafka;

namespace Service;

public class KafkaProducerStrategyOptionsView
{
    public SpammerOptionsView SpammerOptionsView { get; set; } = new SpammerOptionsView();

    [DefaultValue(true)]
    public bool SingletonProducer { get; set; }
    
    [DefaultValue(1_000_000)]
    public int Size { get; set; } = 1_000_000;
    
    [DefaultValue(-1)]
    public int MaxMsgSize { get; set; } = -1;

    [DefaultValue(1)] 
    public int TopicPoolSize { get; set; } = 1;

    [DefaultValue(false)]
    public bool GenerateGuidKey { get; set; } = false;
    
    [DefaultValue(CompressionType.Snappy)]
    public CompressionType CompressionType { get; set; } = CompressionType.Snappy;
    
    [DefaultValue(Acks.All)]
    public Acks Acks { get; set; } = Acks.All;

    public Partitioner? Partitioner { get; set; } = null;

    [DefaultValue(false)]
    public bool DebugLogs { get; set; } = false;
    
    [DefaultValue(false)]
    public bool EnableIdempotence { get; set; } = true;

    [DefaultValue(false)]
    public bool SocketNagleDisable { get; set; } = false;

    [DefaultValue(null)]
    public int? MaxInFlight { get; set; } = null;

    public double? LingerMs { get; set; } = null;

    public int? BatchSize { get; set; } = null;

    public int? BatchNumMessages { get; set; } = null;

    [DefaultValue(0)]
    public int StartupJitterMs { get; set; } = 0;

    public int? SocketReceiveBufferBytes { get; set; } = null;

    public int? SocketSendBufferBytes { get; set; } = null;
}