using Newtonsoft.Json;
using ProtoBuf;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.References.GeneralisedReferences;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.SystemObjects.Interfaces;
using ScenarioModelling.CoreObjects.SystemObjects.Properties;
using ScenarioModelling.CoreObjects.Visitors;
using YamlDotNet.Serialization;

namespace ScenarioModelling.CoreObjects.SystemObjects;

[ProtoContract]
[SystemObjectLike<ISystemObject, Relation>]
public record Relation : ISystemObject<RelationReference>, IStateful // TODO Rename to something more specific that doesn't collide with System.Linq.Expression
{
    private MetaState _system = null!;

    [JsonIgnore]
    [YamlIgnore]
    public Type Type => typeof(Relation);

    [ProtoMember(1)]
    public string Name { get; set; } = "";

    [ProtoMember(2)]
    public RelatableObjectReference? LeftEntity { get; set; }

    [ProtoMember(3)]
    public RelatableObjectReference? RightEntity { get; set; }

    [ProtoMember(4)]
    public StateProperty InitialState { get; private set; }

    [ProtoMember(5)]
    public StateProperty State { get; }

    private Relation()
    {

    }

    public Relation(MetaState system)
    {
        _system = system;

        // Add this to the system
        system.Relations.Add(this);

        InitialState = new StateProperty(system);
        State = new(system);
    }

    public void InitialiseAfterDeserialisation(MetaState system)
    {
        _system = system;
    }

    public RelationReference GenerateReference()
        => new RelationReference(_system) { Name = Name };

    public IStatefulObjectReference GenerateStatefulReference()
        => GenerateReference();

    public object Accept(ISystemVisitor visitor)
        => visitor.VisitRelation(this);
}
