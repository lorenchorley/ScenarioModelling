namespace ScenarioModel.Execution.Events.Factory;

public interface IEventFactory
{
    E CreateEvent<E>(string eventTypeName, Action<E> configure) where E : IScenarioEvent;
}
