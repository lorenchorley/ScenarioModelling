using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CodeHooks.Utils;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;
using ScenarioModelling.Execution.Events;
using ScenarioModelling.Serialisation.Expressions.Interpreter;
using ScenarioModelling.Tools.Exceptions;

namespace ScenarioModelling.CodeHooks.HookDefinitions.StoryObjects;


[StoryNodeLike<INodeHookDefinition, LoopNode>]
public class LoopHookDefinition : IConditionRegistrationNodeHookDefinition<LoopHookDefinition, BifurcatingHook>
{
    private int _loopCount = 0;
    private SubgraphScopedHookSynchroniser? _loopScope;
    private readonly IMetaStoryHookFunctions _hookFunctions;

    [StoryNodeLikeProperty]
    public List<bool> RecordedLoopEvents { get; } = new();

    public bool Validated { get; private set; } = false;
    public LoopNode Node { get; private set; }
    public SubgraphScopedHookSynchroniser Scope { get; }
    public SubGraphScopeSnapshot ScopeSnapshot { get; }

    public LoopHookDefinition(SubgraphScopedHookSynchroniser scope, IMetaStoryHookFunctions hookFunctions)
    {
        _hookFunctions = hookFunctions;
        Scope = scope;
        ScopeSnapshot = Scope.TakeSnapshot();

        Node = new LoopNode();
    }

    private bool LoopHook(bool result)
    {
        _loopCount++;
        bool firstRun = _loopCount == 1;

        RecordedLoopEvents.Add(result);

        // Otherwise on the first run we need to enter the loop's scope
        if (firstRun)
        {
            // This is where it should be, that is when it's used ; once the condition is evoked and not when the hook declaration is made.
            // But only on the first usage for a loop
            _hookFunctions.FinaliseDefinition(this);

            // If the first result is false, we don't enter the loop at all
            if (!result)
                return RegisterEventFromConditionResult(result); // We do not register the event here because it is managed by the call to _finaliseDefinition which also adds the node to the graph

            // Continue the loop
            _loopScope = _hookFunctions.EnterSubgraph(Node.SubGraph);
        }

        if (result)
        {
            // Reset the position in the loop subgraph to the beginning
            ArgumentNullExceptionStandard.ThrowIfNull(_loopScope);
            _loopScope.ReturnToStartOfScope();
        }
        else
        {
            // End the loop and return to the parent scope
            _hookFunctions.ReturnOneScopeLevel();
        }

        RegisterEventFromConditionResult(result);

        return result;
    }

    private bool RegisterEventFromConditionResult(bool result)
    {
        _hookFunctions.RegisterEventForHook(this, e => ((LoopEvent)e).LoopRun = result);
        return result;
    }

    private bool _gotConditionHook = false;

    public LoopHookDefinition GetConditionHook(out BifurcatingHook loopHook)
    {
        _gotConditionHook = true;
        loopHook = LoopHook;
        return this;
    }

    public LoopHookDefinition SetAsImplicit()
    {
        Node.Implicit = true;
        return this;
    }

    public IStoryNode GetNode()
    {
        return Node;
    }

    public void Validate()
    {
        if (!_gotConditionHook)
        {
            throw new HookException($"The hook declaration did not ask for a condition hook callback, call {nameof(GetConditionHook)}");
        }

        Validated = true;
    }

    public void Build()
    {
        Validate();
    }

    public void ReplaceNodeWithExisting(IStoryNode preexistingNode)
    {
        if (preexistingNode is not LoopNode node)
            throw new InternalLogicException($"When trying to replace the hook definition's generated node with a preexisting node, the types did not match (preexisting type : {preexistingNode.GetType().Name}, generated type : {Node.GetType().Name})");

        Node = node;
    }
}
