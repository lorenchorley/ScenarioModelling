using ScenarioModel.SystemObjects.Entities;

namespace ScenarioModel.ScenarioObjects.Events;

public class ChoiceSelectedEvent : IScenarioEvent
{
    public string Choice { get; set; } = "";
    public IScenarioNode ProducerNode { get; init; } = null!;
}
