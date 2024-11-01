using ScenarioModel.Objects.SystemObjects.States;

namespace ScenarioModel.Objects.SystemObjects.Entities;

public record AspectType : INameful
{
    public string Name { get; set; } = "";
    public StateMachine StateMachine { get; set; } = null!;
}
