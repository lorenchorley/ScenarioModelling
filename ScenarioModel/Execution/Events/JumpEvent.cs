using ScenarioModel.ScenarioObjects;

namespace ScenarioModel.Execution.Events;

public record JumpEvent : IScenarioEvent
{
    public string Target { get; init; } = "";
    public IScenarioNode ProducerNode { get; init; } = null!;
}
