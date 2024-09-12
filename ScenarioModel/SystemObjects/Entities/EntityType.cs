using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.SystemObjects.Entities;

/// <summary>
/// Types exist only to allow grouping and reuse of entities (that would then have the same state type and aspects)
/// </summary>
public class EntityType : IStatefulType
{
    public string Name { get; set; } = "";
    public StateType? StateType { get; set; }
    public List<AspectType> AspectTypes { get; set; } = new();
}
