using ScenarioModel.SystemObjects.Entities;

namespace ScenarioModel.ScenarioObjects.Events;

public interface IScenarioEvent
{
    IScenarioNode ProducerNode { get; }
}
