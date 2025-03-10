using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CodeHooks.Utils;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.Tools.Exceptions;

namespace ScenarioModelling.CodeHooks.HookDefinitions.StoryObjects;

[StoryNodeLike<INodeHookDefinition, MetadataNode>]
public class MetadataHookDefinition : IInSituNodeHookDefinition
{
    private readonly IHookFunctions _hookFunctions;

    public bool Validated { get; private set; } = false;
    public MetadataNode Node { get; private set; }
    public SubgraphScopedHookSynchroniser Scope { get; }
    public SubGraphScopeSnapshot ScopeSnapshot { get; }

    public MetadataHookDefinition(SubgraphScopedHookSynchroniser scope, string value, IHookFunctions hookFunctions)
    {
        _hookFunctions = hookFunctions;
        Scope = scope;
        ScopeSnapshot = Scope.TakeSnapshot();

        Node = new MetadataNode()
        {
            Value = value
        };
    }

    public IStoryNode GetNode()
    {
        return Node;
    }

    public MetadataHookDefinition WithKey(string key)
    {
        Node.Key = key;
        return this;
    }

    public MetadataHookDefinition WithType(string type)
    {
        Node.MetadataType = type;
        return this;
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
        if (preexistingNode is not MetadataNode node)
            throw new InternalLogicException($"When trying to replace the hook definition's generated node with a preexisting node, the types did not match (preexisting type : {preexistingNode.GetType().Name}, generated type : {Node.GetType().Name})");

        Node = node;
    }
}
