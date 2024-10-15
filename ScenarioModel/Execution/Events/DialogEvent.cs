using ScenarioModel.References;
using ScenarioModel.ScenarioObjects;

namespace ScenarioModel.Execution.Events;

public record DialogEvent : IScenarioEvent
{
    public string Text { get; set; } = null!;
    public string? Character { get; set; } = null;
    public IScenarioNode ProducerNode { get; init; } = null!;
}
