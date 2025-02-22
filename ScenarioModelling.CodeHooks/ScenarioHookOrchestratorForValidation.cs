using ScenarioModelling.CodeHooks.HookDefinitions;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.Serialisation.ContextConstruction;
using ScenarioModelling.Tools.Exceptions;

namespace ScenarioModelling.CodeHooks;

public class MetaStoryHookOrchestratorForValidation(Context context, Instanciator instanciator, MetaStoryStack metaStoryStack, IServiceProvider serviceProvider) : MetaStoryHookOrchestrator(context, instanciator, metaStoryStack, serviceProvider)
{
    public override MetaStoryHookDefinition? StartMetaStory(string name)
    {
        if (Context.MetaStories.Any(s => s.Name == name))
        {
            throw new HookException($"MetaStory name {name} does not exist in context");
        }

        return base.StartMetaStory(name);
    }
}