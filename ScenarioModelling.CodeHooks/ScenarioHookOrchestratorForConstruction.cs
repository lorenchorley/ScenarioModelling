using ScenarioModelling.CoreObjects;
using ScenarioModelling.Serialisation.ContextConstruction;
namespace ScenarioModelling.CodeHooks;

public class MetaStoryHookOrchestratorForConstruction(Context context, Instanciator instanciator, MetaStoryStack metaStoryStack, IServiceProvider serviceProvider) : MetaStoryHookOrchestrator(context, instanciator, metaStoryStack, serviceProvider)
{
}


