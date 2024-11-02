using ScenarioModel.CodeHooks.HookDefinitions;

namespace ScenarioModel.CodeHooks;

public class HooksForScenarioCreation(Context Context) : Hooks
{
    public override ScenarioHookDefinition? DeclareScenarioStart(string name)
    {
        return _scenarioDefintion = new ScenarioHookDefinition(name, Context);
    }


}


