using ScenarioModelling.CodeHooks.HookDefinitions;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.Serialisation.ContextConstruction;

namespace ScenarioModelling.CodeHooks;

public class MetaStoryHookOrchestratorForValidation(Context context, Instanciator instanciator) : MetaStoryHookOrchestrator(context, instanciator)
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