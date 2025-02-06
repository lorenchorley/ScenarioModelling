using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.Execution.Events;
using ScenarioModelling.Execution.Events.Interfaces;

namespace ScenarioModelling.CodeHooks.Utils;

//public delegate void ReturnOneScopeLevelDelegate();
//public delegate void EnterScopeDelegate(DefinitionScope scope);
//public delegate void FinaliseDefinitionDelegate(INodeHookDefinition hookDefinition);
//public delegate void RegisterEventForHookDelegate(INodeHookDefinition hookDefinition, Action<IStoryEvent> configure);

public delegate string ArbitraryBranchingHook(string result);
public delegate bool BifurcatingHook(bool result);

public delegate void BlockEndHook();
public delegate IDisposable ScopeDefiningHook();