using ScenarioModel.Objects.System;
using ScenarioModel.Objects.System.States;

namespace ScenarioModel.Objects.System.Entities;

public record AspectType : INameful
{
    public string Name { get; set; } = "";
    public StateMachine StateMachine { get; set; } = null!;
}
