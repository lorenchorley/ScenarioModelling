using ScenarioModel.SystemObjects.Relations;
using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.SystemObjects.Entities;

public class Entity : IStateful, IRelatable
{
    public string Name { get; set; } = "";
    public EntityType EntityType { get; set; } = null!;
    public List<Relation> Relations { get; set; } = new();
    public List<Aspect> Aspects { get; set; } = new();
    public State? State { get; set; }
}
