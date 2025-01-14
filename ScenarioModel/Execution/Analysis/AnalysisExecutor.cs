using ScenarioModel.Execution.Events.Interfaces;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.Execution.Analysis;

public class AnalysisExecutor : IExecutor
{
    public IScenarioEvent? GetLastEvent()
    {
        throw new NotImplementedException();
    }

    public bool IsLastEventOfType<T>() where T : IScenarioEvent
    {
        throw new NotImplementedException();
    }

    public bool IsLastEventOfType<T>(Func<T, bool> pred) where T : IScenarioEvent
    {
        throw new NotImplementedException();
    }

    public IScenarioNode? NextNode()
    {
        throw new NotImplementedException();
    }

    public void RegisterEvent(IScenarioEvent @event)
    {
        throw new NotImplementedException();
    }

    public Story StartScenario(string name)
    {
        throw new NotImplementedException();
    }
}
