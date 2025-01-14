using ScenarioModel.Execution.Events.Interfaces;
using ScenarioModel.Expressions.Evaluation;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.Execution.Dialog;

public class DialogExecutor : IExecutor
{
    public Context Context { get; set; }
    private MetaStory? _scenario;
    private Story? _scenarioRun;
    private readonly ExpressionEvalator _evalator;

    public DialogExecutor(Context context, ExpressionEvalator evalator)
    {
        Context = context;
        _evalator = evalator;
    }

    public IScenarioEvent? GetLastEvent()
    {
        if (_scenario == null || _scenarioRun == null)
            throw new InvalidOperationException("Scenario not started");

        return _scenarioRun.Events.LastOrDefault();
    }

    public Story StartScenario(string name)
    {
        Context.ResetToInitialState();

        _scenario = Context.Scenarios.FirstOrDefault(s => s.Name == name);

        if (_scenario == null)
            throw new InvalidOperationException($"No scenario with name {name}");

        _scenarioRun = new Story { Scenario = _scenario, Evaluator = _evalator };
        _scenarioRun.Init();

        return _scenarioRun;
    }

    public IScenarioNode? NextNode()
    {
        if (_scenario == null || _scenarioRun == null)
            throw new InvalidOperationException("Scenario not started");

        return _scenarioRun.NextNode();
    }

    public void RegisterEvent(IScenarioEvent @event)
    {
        if (_scenario == null || _scenarioRun == null)
            throw new InvalidOperationException("Scenario not started");

        _scenarioRun.RegisterEvent(@event);
    }

    public bool IsLastEventOfType<T>() where T : IScenarioEvent
    {
        return IsLastEventOfType<T>(_ => true);
    }

    public bool IsLastEventOfType<T>(Func<T, bool> pred) where T : IScenarioEvent
    {
        IScenarioEvent? scenarioEvent = _scenarioRun?.Events.LastOrDefault();

        if (scenarioEvent == null)
        {
            return false;
        }

        return scenarioEvent is T t && pred(t);
    }
}
