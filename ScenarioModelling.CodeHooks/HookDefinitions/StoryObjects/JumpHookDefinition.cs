using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CodeHooks.Utils;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.Tools.Exceptions;

namespace ScenarioModelling.CodeHooks.HookDefinitions.StoryObjects;

[StoryNodeLike<INodeHookDefinition, JumpNode>]
public class JumpHookDefinition : IInSituNodeHookDefinition
{
    private readonly IHookFunctions _hookFunctions;

    public bool Validated { get; private set; } = false;
    public JumpNode Node { get; private set; }
    public SubgraphScopedHookSynchroniser Scope { get; }
    public SubGraphScopeSnapshot ScopeSnapshot { get; }

    public JumpHookDefinition(SubgraphScopedHookSynchroniser scope, string target, IHookFunctions hookFunctions)
    {
        _hookFunctions = hookFunctions;
        Scope = scope;
        ScopeSnapshot = Scope.TakeSnapshot();

        Node = new JumpNode()
        {
            Target = target
        };
    }

    public JumpHookDefinition SetId(string id)
    {
        Node.Name = id;

        return this;
    }

    public JumpHookDefinition SetAsImplicit()
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
        Validated = true;
    }

    public void BuildAndRegister()
    {
        Validate();
        _hookFunctions.FinaliseDefinition(this);
        _hookFunctions.RegisterEventForHook(this, _ => { });
    }

    public void ReplaceNodeWithExisting(IStoryNode preexistingNode)
    {
        if (preexistingNode is not JumpNode node)
            throw new InternalLogicException($"When trying to replace the hook definition's generated node with a preexisting node, the types did not match (preexisting type : {preexistingNode.GetType().Name}, generated type : {Node.GetType().Name})");

        Node = node;
    }
}
