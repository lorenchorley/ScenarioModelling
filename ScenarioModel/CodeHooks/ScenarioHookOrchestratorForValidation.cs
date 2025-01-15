using ScenarioModelling.CodeHooks.HookDefinitions;

namespace ScenarioModelling.CodeHooks;

public class ScenarioHookOrchestratorForValidation(Context context) : ScenarioHookOrchestrator(context)
{
    public override MetaStoryHookDefinition? StartMetaStory(string name)
    {
        if (Context.Scenarios.Any(s => s.Name == name))
        {
            throw new InvalidOperationException($"Scenario name {name} does not exist in context");
        }

        return base.StartMetaStory(name);
    }
}