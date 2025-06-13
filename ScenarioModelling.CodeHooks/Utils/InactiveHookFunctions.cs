using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;
using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Tools.Collections.Graph;
using ScenarioModelling.Tools.Exceptions;

namespace ScenarioModelling.CodeHooks.Utils;

public class InactiveHookFunctions : IMetaStoryHookFunctions
{
    private readonly Queue<IHookDefinition> _newlyCreatedHooks;

    public InactiveHookFunctions(Queue<IHookDefinition> newlyCreatedHooks)
    {
        _newlyCreatedHooks = newlyCreatedHooks;
    }

    /// <summary>
    /// Validate and add as we go that each node definition is correct so that 
    /// * the problem is raised as close to the definition as possible,
    /// * and so that each node is verified before the next is started.
    /// </summary>
    public void VerifyPreviousDefinition()
    {
        if (_newlyCreatedHooks.Count == 0)
        {
            // Nothing to do
            return;
        }

        if (_newlyCreatedHooks.Count > 1)
            throw new HookException("Only one definition should have been create since the last call");

        IHookDefinition previousDefinition = _newlyCreatedHooks.Dequeue();

        if (!previousDefinition.Validated)
            throw new HookException("Previous definition was not validated, call the Build method to finalise the hook definition");
    }

    public virtual SubgraphScopedHookSynchroniser EnterSubgraph(SemiLinearSubGraph<IStoryNode> subgraph)
    {
        throw new InternalLogicException($"{nameof(EnterSubgraph)} called without starting a meta story via hooks");
    }

    public virtual void ReturnOneScopeLevel()
    {
        throw new InternalLogicException($"{nameof(ReturnOneScopeLevel)} called without starting a meta story via hooks");
    }

    public virtual void FinaliseDefinition(IHookDefinition hookDefinition)
    {
        throw new InternalLogicException($"{nameof(FinaliseDefinition)} called without starting a meta story via hooks");
    }

    public virtual void RegisterEventForHook(INodeHookDefinition hookDefinition, Action<IMetaStoryEvent> configure)
    {
        throw new InternalLogicException($"{nameof(RegisterEventForHook)} called without starting a meta story via hooks");
    }
}
