using ScenarioModel.Objects.ScenarioObjects.BaseClasses;
using ScenarioModel.References;

namespace ScenarioModel.Execution.Events;

public record StateChangeEvent : IScenarioEvent
{
    public IReference StatefulObject { get; set; } = null!;
    public StateReference InitialState { get; set; } = null!;
    public StateReference FinalState { get; set; } = null!;
    public string TransitionName { get; set; } = null!;
    public IScenarioNode ProducerNode { get; init; } = null!;

}
