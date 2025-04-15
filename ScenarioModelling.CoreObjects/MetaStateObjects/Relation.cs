using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.References.GeneralisedReferences;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects.Properties;
using ScenarioModelling.CoreObjects.Visitors;
using YamlDotNet.Serialization;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;

namespace ScenarioModelling.CoreObjects.MetaStateObjects;

[MetaStateObjectLike<IMetaStateObject, Relation>]
public record Relation : IMetaStateObject<RelationReference>, IStateful // TODO Rename to something more specific that doesn't collide with System.Linq.Expression
{
    private MetaState _system = null!;

    [JsonIgnore]
    [YamlIgnore]
    public Type Type => typeof(Relation);

    public string Name { get; set; } = "";

    public RelatableObjectReference? LeftEntity { get; set; }

    public RelatableObjectReference? RightEntity { get; set; }

    public StateProperty InitialState { get; private set; }

    public StateProperty State { get; }

    private Relation()
    {

    }

    public Relation(MetaState system)
    {
        _system = system;

        // Add this to the system
        //system.Relations.Add(this);

        InitialState = new StateProperty(system);
        State = new(system);
    }

    public void InitialiseAfterDeserialisation(MetaState system)
    {
        _system = system;
    }

    public OneOfMetaStateObject ToOneOf()
        => new OneOfMetaStateObject(this);

    public RelationReference GenerateReference()
        => new RelationReference(_system) { Name = Name };

    public IStatefulObjectReference GenerateStatefulReference()
        => GenerateReference();

    public object Accept(IMetaStateVisitor visitor)
        => visitor.VisitRelation(this);
}
