using ScenarioModel.Execution.Events;
using ScenarioModel.Execution.Events.Factory;

namespace ScenarioModel.Execution.Analysis;

public class AnalysisEventFactory : IEventFactory
{
    public E CreateEvent<E>(string eventTypeName, Action<E> configure) where E : IScenarioEvent
    {
        throw new NotImplementedException();
    }
}
