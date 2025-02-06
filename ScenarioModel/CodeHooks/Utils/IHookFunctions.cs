using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.Execution.Events.Interfaces;

namespace ScenarioModelling.CodeHooks.Utils;

public interface IHookFunctions
{
    void VerifyPreviousDefinition();
    void EnterScope(DefinitionScope scope);
    void ReturnOneScopeLevel();
    void FinaliseDefinition(INodeHookDefinition hookDefinition);
    void RegisterEventForHook(INodeHookDefinition hookDefinition, Action<IStoryEvent> configure);
}
