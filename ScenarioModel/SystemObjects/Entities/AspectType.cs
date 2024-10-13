using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.SystemObjects.Entities;

public class AspectType : INameful
{
    public string Name { get; set; } = "";
    public StateType StateType { get; set; } = null!;
}
