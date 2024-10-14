using ScenarioModel.References;
using ScenarioModel.ScenarioObjects;

namespace ScenarioModel.Execution.Events;

public class AddRelationEvent : IScenarioEvent
{
    public IReference First { get; set; } = null!;
    public IReference Second { get; set; } = null!;
    public IScenarioNode ProducerNode { get; init; } = null!;
}
