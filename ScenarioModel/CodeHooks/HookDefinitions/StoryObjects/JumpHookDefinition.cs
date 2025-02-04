using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.StoryNodes;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;

namespace ScenarioModelling.CodeHooks.HookDefinitions.StoryObjects;

[StoryNodeLike<INodeHookDefinition, JumpNode>]
public class JumpHookDefinition : IInSituNodeHookDefinition
{
    private readonly HookFunctions _hookFunctions;

    public bool Validated { get; private set; } = false;
    public JumpNode Node { get; private set; }
    public DefinitionScope Scope { get; }
    public DefinitionScopeSnapshot ScopeSnapshot { get; }

    public JumpHookDefinition(DefinitionScope scope, string target, HookFunctions hookFunctions)
    {
        _hookFunctions = hookFunctions;

        Node = new JumpNode()
        {
            Target = target
        };
        Scope = scope;
        ScopeSnapshot = Scope.TakeSnapshot();
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
            throw new Exception($"When trying to replace the hook definition's generated node with a preexisting node, the types did not match (preexisting type : {preexistingNode.GetType().Name}, generated type : {Node.GetType().Name})");

        Node = node;
    }
}
