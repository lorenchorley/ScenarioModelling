using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.SystemObjects.Entities;

public record AspectType : INameful
{
    public string Name { get; set; } = "";
    public StateMachine StateType { get; set; } = null!;
}
