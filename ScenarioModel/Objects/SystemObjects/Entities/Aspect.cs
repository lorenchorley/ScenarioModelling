using ScenarioModel.Objects.SystemObjects.Properties;
using ScenarioModel.Objects.SystemObjects.Relations;
using ScenarioModel.Objects.SystemObjects.States;
using ScenarioModel.References;

namespace ScenarioModel.Objects.SystemObjects.Entities;

public record Aspect(System System) : IStateful, IRelatable
{
    public string Name
    {
        get => AspectType.Name;
    }

    public AspectType AspectType { get; set; } = null!;
    public Entity Entity { get; set; } = null!;
    public List<Relation> Relations { get; set; } = new();
    public StateProperty State { get; } = new(System);

    public IStatefulObjectReference GenerateReference()
    {
        return new EntityAspectReference() { AspectName = Name };
    }

    public bool IsEqv(Aspect other)
    {
        return true; // TODO
    }
}
