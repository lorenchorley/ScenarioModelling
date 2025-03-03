﻿using ScenarioModelling.CoreObjects;
using ScenarioModelling.Serialisation.ContextConstruction;
namespace ScenarioModelling.CodeHooks;

public class MetaStoryHookOrchestratorForConstruction(Context context, Instanciator instanciator, MetaStoryDefinitionStack metaStoryStack, IServiceProvider serviceProvider) : MetaStoryHookOrchestrator(context, instanciator, metaStoryStack, serviceProvider)
{
}


