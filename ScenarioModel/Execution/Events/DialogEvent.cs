using ScenarioModel.References;
using ScenarioModel.ScenarioObjects;

namespace ScenarioModel.Execution.Events;

public class DialogEvent : IScenarioEvent
{
    public string Text { get; set; } = null!;
    public IScenarioNode ProducerNode { get; init; } = null!;
}
