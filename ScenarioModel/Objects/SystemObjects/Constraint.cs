using Newtonsoft.Json;
using ProtoBuf;
using ScenarioModelling.Execution.Events;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Expressions.SemanticTree;
using ScenarioModelling.Objects.SystemObjects.Interfaces;
using ScenarioModelling.Objects.Visitors;
using ScenarioModelling.References;
using YamlDotNet.Serialization;

namespace ScenarioModelling.Objects.SystemObjects;

[ProtoContract]
[SystemObjectLike<ISystemObject, Constraint>]
public record Constraint : ISystemObject<ConstraintReference>
{
    private System _system = null!;

    [JsonIgnore]
    [YamlIgnore]
    public Type Type => typeof(Constraint);

    [ProtoMember(1)]
    public string Name { get; set; } = "";

    [ProtoMember(2)]
    [SystemObjectLikeProperty(serialise: false)]
    public Expression Condition { get; set; } = null!;

    [ProtoMember(3)]
    [SystemObjectLikeProperty(serialise: false)]
    public string OriginalConditionText { get; set; } = "";
    
    private Constraint()
    {
        
    }

    public Constraint(System system)
    {
        _system = system;

        // Add this to the system
        system.Constraints.Add(this);
    }

    public void InitialiseAfterDeserialisation(System system)
    {
        _system = system;
    }

    public ConstraintReference GenerateReference()
        => new ConstraintReference(_system) { Name = Name };

    public object Accept(ISystemVisitor visitor)
        => visitor.VisitConstraint(this);

    public ConstraintFailedEvent GenerateConstraintFailedEvent()
    {
        return new ConstraintFailedEvent() { ProducerNode = this, Expression = OriginalConditionText };
    }
}
