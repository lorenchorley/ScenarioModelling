using ScenarioModel.Objects.ScenarioObjects;

namespace ScenarioModel.Execution.Events;

public record WhileLoopConditionCheckEvent : IScenarioEvent<WhileNode>
{
    public bool LoopBlockRun { get; set; }
    public WhileNode ProducerNode { get; init; } = null!;
}
