using ScenarioModel.Objects.ScenarioObjects;

namespace ScenarioModel.Execution.Events;

public record ChoiceSelectedEvent : IScenarioEvent<ChooseNode>
{
    public string Choice { get; set; } = "";
    public ChooseNode ProducerNode { get; init; } = null!;
}
