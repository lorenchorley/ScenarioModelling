using ScenarioModel.References;

namespace ScenarioModel.Objects.SystemObjects.Constraints;

public record Constraint
{
    public string Name { get; set; } = "";

    public IStatefulObjectReference GenerateReference()
    {
        return new EntityAspectReference() { AspectName = Name };
    }
}
