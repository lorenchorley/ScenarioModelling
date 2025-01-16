using Newtonsoft.Json;
using ScenarioModelling.Execution.Events;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Expressions.SemanticTree;
using ScenarioModelling.Objects.SystemObjects.Interfaces;
using ScenarioModelling.Objects.Visitors;
using ScenarioModelling.References;

namespace ScenarioModelling.Objects.SystemObjects;

[SystemObjectLike<ISystemObject, Constraint>]
public record Constraint : ISystemObject<ConstraintReference>
{
    private readonly System _system;

    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(Constraint);

    [SystemObjectLikeProperty(serialise: false)]
    public Expression Condition { get; set; } = null!;

    [SystemObjectLikeProperty(serialise: false)]
    public string OriginalConditionText { get; set; } = "";

    public Constraint(System system)
    {
        _system = system;

        // Add this to the system
        system.Constraints.Add(this);
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
