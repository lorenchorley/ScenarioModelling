using Newtonsoft.Json;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects.Interfaces;
using ScenarioModelling.Objects.SystemObjects.Properties;
using ScenarioModelling.Objects.Visitors;
using ScenarioModelling.References;
using ScenarioModelling.References.GeneralisedReferences;
using ScenarioModelling.References.Interfaces;

namespace ScenarioModelling.Objects.SystemObjects;

[ObjectLike<ISystemObject, Relation>]
public record Relation : ISystemObject<RelationReference>, IStateful
{
    private readonly System _system;

    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(Relation);

    public RelatableObjectReference? LeftEntity { get; set; } // TODO Propertyise
    public RelatableObjectReference? RightEntity { get; set; } // TODO Propertyise
    public StateProperty InitialState { get; private set; }
    public StateProperty State { get; }

    public Relation(System system)
    {
        _system = system;

        // Add this to the system
        system.Relations.Add(this);

        InitialState = new StateProperty(system);
        State = new(system);
    }

    public RelationReference GenerateReference()
        => new RelationReference(_system) { Name = Name };

    public IStatefulObjectReference GenerateStatefulReference()
        => GenerateReference();

    public object Accept(ISystemVisitor visitor)
        => visitor.VisitRelation(this);
}
