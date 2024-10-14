using ScenarioModel.ScenarioObjects;

namespace ScenarioModel.Execution.Events;

public interface IScenarioEvent
{
    IScenarioNode ProducerNode { get; }
}
