using Newtonsoft.Json;
using ProtoBuf;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.SystemObjects.Interfaces;
using ScenarioModelling.CoreObjects.SystemObjects.Properties;
using ScenarioModelling.CoreObjects.Visitors;
using YamlDotNet.Serialization;

namespace ScenarioModelling.CoreObjects.SystemObjects;

[ProtoContract]
[SystemObjectLike<ISystemObject, Aspect>]
public record Aspect : ISystemObject<AspectReference>, IStateful, IRelatable
{
    private MetaState _system = null!;

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

    public Aspect(MetaState system)
    {
        _system = system;

        // Add this to the system
        system.Aspects.Add(this);

        Entity = new EntityProperty(system);
        InitialState = new StateProperty(system);
        State = new StateProperty(system);
        Relations = new RelationListProperty(system);
    }

    public void InitialiseAfterDeserialisation(MetaState system)
    {
        _system = system;
    }

    public AspectReference GenerateReference()
        => new AspectReference(_system) { Name = Name };

    public IStatefulObjectReference GenerateStatefulReference()
        => new AspectReference(_system) { Name = Name };

    IRelatableObjectReference ISystemObject<IRelatableObjectReference>.GenerateReference()
        => GenerateReference();

    IRelatableObjectReference IReferencable<IRelatableObjectReference>.GenerateReference()
        => GenerateReference();

    public object Accept(ISystemVisitor visitor)
        => visitor.VisitAspect(this);

}
