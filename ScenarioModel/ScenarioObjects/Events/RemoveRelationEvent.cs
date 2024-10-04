using ScenarioModel.References;
using ScenarioModel.SystemObjects.Entities;

namespace ScenarioModel.ScenarioObjects.Events;

public class RemoveRelationEvent : IScenarioEvent
{
    public RelationReference Ref { get; set; } = null!;
    public IScenarioNode ProducerNode { get; init; } = null!;
}
