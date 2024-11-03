namespace ScenarioModel.CodeHooks.HookDefinitions;

public class ScenarioHookDefinition
{
    public string Name { get; }
    public Context Context { get; }

    private Scenario _activeScenario;

    public ScenarioHookDefinition(string Name, Context Context)
    {
        this.Name = Name;
        this.Context = Context;

        Scenario? scenario = Context.Scenarios.FirstOrDefault(s => s.Name == Name);

        if (scenario == null)
        {
            scenario = new Scenario() { Name = Name, System = Context.System };
            Context.Scenarios.Add(scenario);
        }

        _activeScenario = scenario;
    }

    public Scenario GetScenario()
    {
        return _activeScenario;
    }
}
