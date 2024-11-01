using ScenarioModel.Objects.ScenarioObjects;

namespace ScenarioModel.Execution.Events;

public record DialogEvent : IScenarioEvent<DialogNode>
{
    public string Text { get; set; } = null!;
    public string? Character { get; set; } = null;
    public DialogNode ProducerNode { get; init; } = null!;
}
