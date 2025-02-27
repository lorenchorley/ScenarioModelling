using Newtonsoft.Json;
using ProtoBuf;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.Expressions.SemanticTree;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;
using ScenarioModelling.CoreObjects.Visitors;
using YamlDotNet.Serialization;

namespace ScenarioModelling.CoreObjects.MetaStateObjects;

[ProtoContract]
[MetaStateObjectLike<ISystemObject, Constraint>]
public record Constraint : ISystemObject<ConstraintReference>
{
    private MetaState _system = null!;

    [JsonIgnore]
    [YamlIgnore]
    public Type Type => typeof(Constraint);

    [ProtoMember(1)]
    public string Name { get; set; } = "";

    [ProtoMember(2)]
    [MetaStateObjectLikeProperty(serialise: false)]
    public Expression Condition { get; set; } = null!;

    [ProtoMember(3)]
    [MetaStateObjectLikeProperty(serialise: false)]
    public string OriginalConditionText { get; set; } = "";

    private Constraint()
    {

    }

    public Constraint(MetaState system)
    {
        _system = system;

        // Add this to the system
        system.Constraints.Add(this);
    }

    public void InitialiseAfterDeserialisation(MetaState system)
    {
        _system = system;
    }

    public ConstraintReference GenerateReference()
        => new ConstraintReference(_system) { Name = Name };

    public object Accept(IMetaStateVisitor visitor)
        => visitor.VisitConstraint(this);

}
