//using Newtonsoft.Json;
//using ProtoBuf;
//using ScenarioModelling.Annotations.Attributes;
//using ScenarioModelling.CoreObjects.References;
//using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;
//using ScenarioModelling.CoreObjects.MetaStateObjects.Properties;
//using ScenarioModelling.CoreObjects.Visitors;
//using YamlDotNet.Serialization;

//namespace ScenarioModelling.CoreObjects.MetaStateObjects;

//[ProtoContract]
//[MetaStateObjectLike<ISystemObject, MetadataObject>]
//public record MetadataObject : ISystemObject<MetadataObjectReference>, IEqualityComparer<MetadataObject>
//{
//    private MetaState _metaState = null!;

//    [JsonIgnore]
//    [YamlIgnore]
//    public Type Type => typeof(MetadataObject);

//    [ProtoMember(1)]
//    public string Name { get; set; } = "";

//    [ProtoMember(2)]
//    public StateProperty SourceState { get; private set; }

//    [ProtoMember(3)]
//    public StateProperty DestinationState { get; private set; }

//    private MetadataObject()
//    {

//    }

//    public MetadataObject(MetaState metaState)
//    {
//        _metaState = metaState;

//        // Add this to the system
//        _metaState.Metadata.Add(this);

//        SourceState = new(metaState);
//        DestinationState = new(metaState);
//    }

//    public void InitialiseAfterDeserialisation(MetaState metaState)
//    {
//        _metaState = metaState;
//    }

//    public MetadataObjectReference GenerateReference()
//        => new MetadataObjectReference(_metaState)
//        {
//            Name = Name,
//            SourceName = SourceState.Name,
//            DestinationName = DestinationState.Name
//        };

//    public bool IsDeepEqv(MetadataObject other)
//    {
//        return Name.IsEqv(other.Name) &&
//               SourceState.IsEqv(other.SourceState) &&
//               DestinationState.IsEqv(other.DestinationState);
//    }

//    public object Accept(IMetaStateVisitor visitor)
//        => visitor.VisitMetadataObject(this);

//    public bool Equals(MetadataObject? x, MetadataObject? y)
//    {
//        if (x == null || y == null)
//        {
//            if (x != null || y != null)
//                return false; // If only one is null
//            else
//                return true; // If both are null
//        }

//        return x.Name.IsEqv(y.Name) &&
//               x.SourceState.Name.IsEqvCountingNulls(y.SourceState.Name) &&
//               x.DestinationState.Name.IsEqvCountingNulls(y.DestinationState.Name);
//    }

//    public int GetHashCode(/*[DisallowNull]*/ MetadataObject obj)
//        => obj.Name.GetHashCode();
//}