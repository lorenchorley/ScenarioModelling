using ScenarioModel.ScenarioObjects;

namespace ScenarioModel.Execution.Events;

public class ChoiceSelectedEvent : IScenarioEvent
{
    public string Choice { get; set; } = "";
    public IScenarioNode ProducerNode { get; init; } = null!;
}
