using Newtonsoft.Json;
using ProtoBuf;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects.Interfaces;
using ScenarioModelling.Objects.SystemObjects.Properties;
using ScenarioModelling.Objects.Visitors;
using ScenarioModelling.References;
using ScenarioModelling.References.GeneralisedReferences;
using ScenarioModelling.References.Interfaces;
using YamlDotNet.Serialization;

namespace ScenarioModelling.Objects.SystemObjects;

[ProtoContract]
[SystemObjectLike<ISystemObject, Relation>]
public record Relation : ISystemObject<RelationReference>, IStateful
{
    private System _system = null!;

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

    public Relation(System system)
    {
        _system = system;

        // Add this to the system
        system.Relations.Add(this);

        InitialState = new StateProperty(system);
        State = new(system);
    }

    public void InitialiseAfterDeserialisation(System system)
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
