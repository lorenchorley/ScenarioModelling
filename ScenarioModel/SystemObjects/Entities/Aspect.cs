using ScenarioModel.References;
using ScenarioModel.SystemObjects.Relations;
using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.SystemObjects.Entities;

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
