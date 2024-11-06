using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.References;

namespace ScenarioModel.Objects.SystemObjects.Constraints;

public record Constraint
{
    public string Name { get; set; } = "";

    public Expression Condition { get; set; } = null!;

    public IStatefulObjectReference GenerateReference()
    {
        return new EntityAspectReference() { AspectName = Name };
    }
}
