using ScenarioModel.References;

namespace ScenarioModel.ScenarioObjects.Events;

public class RemoveRelationEvent : IScenarioEvent
{
    public RelationReference Ref { get; set; } = null!;
}
