using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.Execution.Events.Interfaces;

public interface IScenarioEvent
{

}

public interface IScenarioEvent<T> : IScenarioEvent
    where T : IScenarioNode
{
    T ProducerNode { get; }
}
