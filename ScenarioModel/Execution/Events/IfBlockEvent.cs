using ScenarioModel.Objects.ScenarioNodes;

namespace ScenarioModel.Execution.Events;

public record IfBlockEvent : IScenarioEvent<IfNode>
{
    public bool IfBlockRun { get; set; }
    public IfNode ProducerNode { get; init; } = null!;
}
