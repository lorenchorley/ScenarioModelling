using ScenarioModel.References;

namespace ScenarioModel.Objects.System.Constraints;

public record Constraint
{
    public string Name { get; set; } = "";

    public IStatefulObjectReference GenerateReference()
    {
        return new EntityAspectReference() { AspectName = Name };
    }
}
