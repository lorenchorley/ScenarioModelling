using ScenarioModelling.CodeHooks.HookDefinitions;

namespace ScenarioModelling.CodeHooks;

public class MetaStoryHookOrchestratorForValidation(Context context) : MetaStoryHookOrchestrator(context)
{
    public override MetaStoryHookDefinition? StartMetaStory(string name)
    {
        if (Context.MetaStories.Any(s => s.Name == name))
        {
            throw new InvalidOperationException($"MetaStory name {name} does not exist in context");
        }

        return base.StartMetaStory(name);
    }
}