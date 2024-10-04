using ScenarioModel.References;
using ScenarioModel.SystemObjects.Entities;

namespace ScenarioModel.ScenarioObjects.Events;

public class StateChangeEvent : IScenarioEvent
{
    public IReference StatefulObject { get; set; } = null!;
    public StateReference InitialState { get; set; } = null!;
    public StateReference FinalState { get; set; } = null!;
    public IScenarioNode ProducerNode { get; init; } = null!;

}
