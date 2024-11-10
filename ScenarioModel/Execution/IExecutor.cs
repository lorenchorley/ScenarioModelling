using ScenarioModel.Execution.Events;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.Execution;

public interface IExecutor
{
    IScenarioEvent? GetLastEvent();
    bool IsLastEventOfType<T>() where T : IScenarioEvent;
    bool IsLastEventOfType<T>(Func<T, bool> pred) where T : IScenarioEvent;
    IScenarioNode? NextNode();
    void RegisterEvent(IScenarioEvent @event);
    ScenarioRun StartScenario(string name);
}
