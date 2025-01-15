using ScenarioModelling.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModelling.Execution.Events.Interfaces;

public interface IScenarioEvent
{

}

public interface IScenarioEvent<T> : IScenarioEvent
    where T : IScenarioNode
{
    T ProducerNode { get; }
}
