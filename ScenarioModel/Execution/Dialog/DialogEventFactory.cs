using ScenarioModel.Execution.Events;
using ScenarioModel.Execution.Events.Factory;
using ScenarioModel.ScenarioObjects;

namespace ScenarioModel.Execution.Dialog;

public class AnalysisEventFactory : IEventFactory
{
    public E CreateEvent<E>(string eventTypeName, Action<E> configure) where E : IScenarioEvent, new()
    {
        switch (eventTypeName)
        {
            case nameof(ChooseNode):
                if (typeof(E) != typeof(ChoiceSelectedEvent))
                {
                    throw new InvalidOperationException();
                }

                var eventInstance = new E();
                configure(eventInstance);
                return eventInstance;
            default:
                throw new NotImplementedException();
        }
    }
}
