using ScenarioModel.References;
using ScenarioModel.SystemObjects.Entities;

namespace ScenarioModel.ScenarioObjects.Events;

public class DialogEvent : IScenarioEvent
{
    public string Text { get; set; } = null!;
    public IScenarioNode ProducerNode { get; init; } = null!;
}
