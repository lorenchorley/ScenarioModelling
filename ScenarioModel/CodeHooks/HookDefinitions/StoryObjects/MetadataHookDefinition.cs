﻿using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CodeHooks.Utils;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.StoryNodes;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;

namespace ScenarioModelling.CodeHooks.HookDefinitions.StoryObjects;

[StoryNodeLike<INodeHookDefinition, MetadataNode>]
public class MetadataHookDefinition : IInSituNodeHookDefinition
{
    private readonly IHookFunctions _hookFunctions;

    public bool Validated { get; private set; } = false;
    public MetadataNode Node { get; private set; }
    public DefinitionScope Scope { get; }
    public DefinitionScopeSnapshot ScopeSnapshot { get; }

    public MetadataHookDefinition(DefinitionScope scope, string value, IHookFunctions hookFunctions)
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
            throw new Exception($"When trying to replace the hook definition's generated node with a preexisting node, the types did not match (preexisting type : {preexistingNode.GetType().Name}, generated type : {Node.GetType().Name})");

        Node = node;
    }
}
