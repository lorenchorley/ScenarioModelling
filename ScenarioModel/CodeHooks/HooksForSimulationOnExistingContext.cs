using ScenarioModel.CodeHooks.HookDefinitions;

namespace ScenarioModel.CodeHooks;

public class HooksForSimulationOnExistingContext(Context Context) : Hooks
{
    public override ScenarioHookDefinition DeclareScenarioStart(string name)
    {
        if (Context.Scenarios.Any(s => s.Name == name))
        {
            throw new InvalidOperationException($"Scenario name {name} does not exist in context");
        }

        return _scenarioDefintion = new ScenarioHookDefinition(name, Context);
    }

}


