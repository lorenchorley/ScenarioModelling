using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModelling.Execution;

public interface IExecutor
{
    IScenarioEvent? GetLastEvent();
    bool IsLastEventOfType<T>() where T : IScenarioEvent;
    bool IsLastEventOfType<T>(Func<T, bool> pred) where T : IScenarioEvent;
    IScenarioNode? NextNode();
    void RegisterEvent(IScenarioEvent @event);
    Story StartScenario(string name);
}
