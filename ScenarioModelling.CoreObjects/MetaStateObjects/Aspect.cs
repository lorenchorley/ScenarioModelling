using Newtonsoft.Json;
using ProtoBuf;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects.Properties;
using ScenarioModelling.CoreObjects.Visitors;
using YamlDotNet.Serialization;

namespace ScenarioModelling.CoreObjects.MetaStateObjects;

[ProtoContract]
[MetaStateObjectLike<ISystemObject, Aspect>]
public record Aspect : ISystemObject<AspectReference>, IStateful, IRelatable
{
    private MetaState _metaState = null!;

    [JsonIgnore]
    [YamlIgnore]
    public Type Type => typeof(Aspect);

    [ProtoMember(1)]
    public string Name { get; set; } = "";

    [ProtoMember(2)]
    public AspectType? AspectType { get; set; }

    [ProtoMember(3)]
    public EntityProperty Entity { get; private set; }

    [ProtoMember(4)]
    public RelationListProperty Relations { get; private set; }

    [ProtoMember(5)]
    public StateProperty InitialState { get; private set; }

    [ProtoMember(6)]
    public StateProperty State { get; private set; }

    private Aspect()
    {

    }

    public Aspect(MetaState metaState)
    {
        _metaState = metaState;

        // Add this to the system
        metaState.Aspects.Add(this);

        Entity = new EntityProperty(metaState);
        InitialState = new StateProperty(metaState);
        State = new StateProperty(metaState);
        Relations = new RelationListProperty(metaState);
    }

    public void InitialiseAfterDeserialisation(MetaState system)
    {
        _metaState = system;
    }

    public AspectReference GenerateReference()
        => new AspectReference(_metaState) { Name = Name };

    public IStatefulObjectReference GenerateStatefulReference()
        => new AspectReference(_metaState) { Name = Name };

    IRelatableObjectReference ISystemObject<IRelatableObjectReference>.GenerateReference()
        => GenerateReference();

    IRelatableObjectReference IReferencable<IRelatableObjectReference>.GenerateReference()
        => GenerateReference();

    public object Accept(IMetaStateVisitor visitor)
        => visitor.VisitAspect(this);

}
