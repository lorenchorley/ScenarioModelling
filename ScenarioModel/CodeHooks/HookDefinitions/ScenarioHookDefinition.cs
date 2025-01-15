namespace ScenarioModelling.CodeHooks.HookDefinitions;

public class ScenarioHookDefinition
{
    public string Name { get; }
    public Context Context { get; }

    private MetaStory _activeScenario;

    public ScenarioHookDefinition(string Name, Context Context)
    {
        this.Name = Name;
        this.Context = Context;

        MetaStory? scenario = Context.Scenarios.FirstOrDefault(s => s.Name == Name);

        if (scenario == null)
        {
            scenario = new MetaStory() { Name = Name, System = Context.System };
            Context.Scenarios.Add(scenario);
        }

        _activeScenario = scenario;
    }

    public MetaStory GetScenario()
    {
        return _activeScenario;
    }
}
