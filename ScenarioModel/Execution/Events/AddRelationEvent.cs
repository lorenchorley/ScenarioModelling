using ScenarioModel.Objects.Scenario;
using ScenarioModel.References;

namespace ScenarioModel.Execution.Events;

public record AddRelationEvent : IScenarioEvent
{
    public IReference First { get; set; } = null!;
    public IReference Second { get; set; } = null!;
    public IScenarioNode ProducerNode { get; init; } = null!;
}
