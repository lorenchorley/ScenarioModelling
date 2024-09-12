using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.SystemObjects.Entities;

public class AspectType
{
    public string Name { get; set; } = "";
    public StateType StateType { get; set; } = null!;
}
