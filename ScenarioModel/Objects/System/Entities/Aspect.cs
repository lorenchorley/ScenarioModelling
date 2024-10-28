using ScenarioModel.Objects.System.Relations;
using ScenarioModel.Objects.System.States;
using ScenarioModel.References;

namespace ScenarioModel.Objects.System.Entities;

public record Aspect : IStateful, IRelatable
{
    public string Name
    {
        get => AspectType.Name;
    }

    public AspectType AspectType { get; set; } = null!;
    public Entity Entity { get; set; } = null!;
    public List<Relation> Relations { get; set; } = new();
    public State? State { get; set; }

    public IStatefulObjectReference GenerateReference()
    {
        return new EntityAspectReference() { AspectName = Name };
    }
}
