using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CodeHooks.Utils;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.Tools.Exceptions;

namespace ScenarioModelling.CodeHooks.HookDefinitions.StoryObjects;


[StoryNodeLike<INodeHookDefinition, ChooseNode>]
public class ChooseHookDefinition : IConditionRegistrationNodeHookDefinition<ChooseHookDefinition, ArbitraryBranchingHook>
{
    private readonly IMetaStoryHookFunctions _hookFunctions;

    [StoryNodeLikeProperty]
    public List<string> RecordedChooseEvents { get; } = new();

    public bool Validated { get; private set; } = false;
    public ChooseNode Node { get; private set; }
    public SubgraphScopedHookSynchroniser Scope { get; }
    public SubGraphScopeSnapshot ScopeSnapshot { get; }

    public ChooseHookDefinition(SubgraphScopedHookSynchroniser scope, IMetaStoryHookFunctions hookFunctions)
    {
        _hookFunctions = hookFunctions;
        Scope = scope;
        ScopeSnapshot = Scope.TakeSnapshot();

        Node = new ChooseNode();
    }

    private string ChooseHook(string result)
    {
        // This is where it should be, that is when it's used ; once the condition is evoked and not when the hook declaration is made.
        _hookFunctions.FinaliseDefinition(this);
        _hookFunctions.RegisterEventForHook(this, _ => { });

        RecordedChooseEvents.Add(result);
        return result;
    }

    private bool _gotConditionHook = false;
    public ChooseHookDefinition GetConditionHook(out ArbitraryBranchingHook hook)
    {
        _gotConditionHook = true;
        hook = ChooseHook;
        return this;
    }

    public ChooseHookDefinition SetId(string id)
    {
        Node.Name = id;
        return this;
    }

    public ChooseHookDefinition WithJump(string nodeId, string choiceName)
    {
        Node.Choices.Add((nodeId, choiceName));
        return this;
    }

    public ChooseHookDefinition SetAsImplicit()
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
            throw new Exception($"The hook declaration did not ask for a condition hook callback, call {nameof(GetConditionHook)}");
        }

        Validated = true;
    }

    public void Build()
    {
        Validate();
    }

    public void ReplaceNodeWithExisting(IStoryNode preexistingNode)
    {
        if (preexistingNode is not ChooseNode node)
            throw new InternalLogicException($"When trying to replace the hook definition's generated node with a preexisting node, the types did not match (preexisting type : {preexistingNode.GetType().Name}, generated type : {Node.GetType().Name})");

        Node = node;
    }
}
