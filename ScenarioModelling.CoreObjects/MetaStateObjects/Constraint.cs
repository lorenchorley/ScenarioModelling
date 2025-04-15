using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.Expressions.SemanticTree;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;
using ScenarioModelling.CoreObjects.Visitors;
using YamlDotNet.Serialization;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;

namespace ScenarioModelling.CoreObjects.MetaStateObjects;

[MetaStateObjectLike<IMetaStateObject, Constraint>]
public record Constraint : IMetaStateObject<ConstraintReference>
{
    private MetaState _system = null!;

    [JsonIgnore]
    [YamlIgnore]
    public Type Type => typeof(Constraint);

    public string Name { get; set; } = "";

    [MetaStateObjectLikeProperty(serialise: false)]
    public Expression Condition { get; set; } = null!;

    [MetaStateObjectLikeProperty(serialise: false)]
    public string OriginalConditionText { get; set; } = "";

    private Constraint()
    {

    }

    public Constraint(MetaState system)
    {
        _system = system;

        // Add this to the system
        //system.Constraints.Add(this);
    }

    public void InitialiseAfterDeserialisation(MetaState system)
    {
        _system = system;
    }

    public OneOfMetaStateObject ToOneOf()
        => new OneOfMetaStateObject(this);

    public ConstraintReference GenerateReference()
        => new ConstraintReference(_system) { Name = Name };

    public object Accept(IMetaStateVisitor visitor)
        => visitor.VisitConstraint(this);

}
