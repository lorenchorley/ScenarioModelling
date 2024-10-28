using ScenarioModel.Objects.Scenario;

namespace ScenarioModel.Execution.Events;

public interface IScenarioEvent
{
    IScenarioNode ProducerNode { get; }
}
