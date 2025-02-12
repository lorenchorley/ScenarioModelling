using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.Execution.Events.Interfaces;

namespace ScenarioModelling.CodeHooks.Utils;

public class InactiveHookFunctions : IHookFunctions
{
    private readonly Queue<INodeHookDefinition> _newlyCreatedHooks;

    public InactiveHookFunctions(Queue<INodeHookDefinition> newlyCreatedHooks)
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
            throw new Exception("Only one definition should have been create since the last call");

        INodeHookDefinition previousDefinition = _newlyCreatedHooks.Dequeue();

        if (!previousDefinition.Validated)
            throw new Exception("Previous definition was not validated, call the Build method to finalise the hook definition");
    }

    public virtual void EnterScope(DefinitionScope scope)
    {
        throw new InvalidOperationException();
    }

    public virtual void ReturnOneScopeLevel()
    {
        throw new InvalidOperationException();
    }

    public virtual void FinaliseDefinition(INodeHookDefinition hookDefinition)
    {
        throw new InvalidOperationException();
    }

    public virtual void RegisterEventForHook(INodeHookDefinition hookDefinition, Action<IStoryEvent> configure)
    {
        throw new InvalidOperationException();
    }
}
