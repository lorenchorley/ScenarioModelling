using ScenarioModel.SystemObjects.Relations;
using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.SystemObjects.Entities;

public class Aspect : IStateful, IRelatable
{
    public string Name 
    { 
        get => Entity.Name + "." + AspectType.Name;
    }

    public AspectType AspectType { get; set; } = null!;
    public Entity Entity { get; set; } = null!;
    public List<Relation> Relations { get; set; } = new();
    public State? State { get; set; }
}
