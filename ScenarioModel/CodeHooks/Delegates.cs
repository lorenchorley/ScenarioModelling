using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;

namespace ScenarioModelling.CodeHooks;

public delegate void ReturnOneScopeLevelDelegate();
public delegate void EnterScopeDelegate(DefinitionScope scope);
public delegate void FinaliseDefinitionDelegate(INodeHookDefinition hookDefinition);

public delegate string ArbitraryBranchingHook(string result);
public delegate bool BifurcatingHook(bool result);

public delegate void BlockEndHook();
public delegate IDisposable ScopeDefiningHook();