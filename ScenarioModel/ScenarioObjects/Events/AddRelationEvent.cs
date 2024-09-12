using ScenarioModel.References;

namespace ScenarioModel.ScenarioObjects.Events;

public class AddRelationEvent : IScenarioEvent
{
    public IReference First { get; set; } = null!;
    public IReference Second { get; set; } = null!;
}
