using ScenarioModel.ScenarioObjects;

namespace ScenarioModel.Execution.Events;

public record IfBlockEvent : IScenarioEvent
{
    public bool IfBlockRun { get; set; }
    public IScenarioNode ProducerNode { get; init; } = null!;
}
