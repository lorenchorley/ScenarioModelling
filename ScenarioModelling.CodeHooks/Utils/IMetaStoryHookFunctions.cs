﻿using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Tools.Collections.Graph;

namespace ScenarioModelling.CodeHooks.Utils;

public interface IMetaStoryHookFunctions
{
    void VerifyPreviousDefinition();
    SubgraphScopedHookSynchroniser EnterSubgraph(SemiLinearSubGraph<IStoryNode> subgraph);
    void ReturnOneScopeLevel();
    void FinaliseDefinition(INodeHookDefinition hookDefinition);
    void RegisterEventForHook(INodeHookDefinition hookDefinition, Action<IMetaStoryEvent> configure);
}
