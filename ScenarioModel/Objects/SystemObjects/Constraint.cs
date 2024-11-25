using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.Objects.Visitors;
using ScenarioModel.References;
using System.Text.Json.Serialization;

namespace ScenarioModel.Objects.SystemObjects;

public record Constraint : ISystemObject<ConstraintReference>
{
    private readonly System _system;

    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(Constraint);

    public Expression Condition { get; set; } = null!;


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
}
