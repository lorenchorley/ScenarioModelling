using ScenarioModel.References;
using ScenarioModel.ScenarioObjects;

namespace ScenarioModel.Execution.Events;

public record RemoveRelationEvent : IScenarioEvent
{
    public RelationReference Ref { get; set; } = null!;
    public IScenarioNode ProducerNode { get; init; } = null!;
}
