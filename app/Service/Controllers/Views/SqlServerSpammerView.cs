using System.Runtime.Serialization;

namespace Service;

[DataContract]
public class SqlServerSpammerView : SpammerBaseView
{
    [DataMember(Name = "entityType")]
    public SqlServerSpammerEntityType EntityType { get; set; }
}