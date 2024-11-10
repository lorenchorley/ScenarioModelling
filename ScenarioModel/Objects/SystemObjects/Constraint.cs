using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.Objects.SystemObjects.Interfaces;

namespace ScenarioModel.Objects.SystemObjects;

public record Constraint : IIdentifiable
{
    private readonly System _system;

    public string Name { get; set; } = "";
    public Type Type => typeof(Constraint);

    public Constraint(System system)
    {
        _system = system;

        // Add this to the system
        system.Constraints.Add(this);
    }


    public Expression Condition { get; set; } = null!;

}
