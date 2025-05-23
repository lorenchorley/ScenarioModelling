﻿using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CodeHooks.Utils;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.References.GeneralisedReferences;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.Tools.Exceptions;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;

namespace ScenarioModelling.CodeHooks.HookDefinitions.StoryObjects;

[StoryNodeLike<INodeHookDefinition, TransitionNode>]
public class TransitionHookDefinition : IInSituNodeHookDefinition
{
    private readonly IMetaStoryHookFunctions _hookFunctions;

    public bool Validated { get; private set; } = false;
    public TransitionNode Node { get; private set; }
    public SubgraphScopedHookSynchroniser Scope { get; }
    public SubGraphScopeSnapshot ScopeSnapshot { get; }

    public TransitionHookDefinition(SubgraphScopedHookSynchroniser scope, MetaState System, string StatefulObjectName, string Transition, IMetaStoryHookFunctions hookFunctions)
    {
        _hookFunctions = hookFunctions;
        Scope = scope;
        ScopeSnapshot = Scope.TakeSnapshot();

        Node = new TransitionNode()
        {
            StatefulObject = new StatefulObjectReference(System) { Name = StatefulObjectName },
            TransitionName = Transition
        };
    }

    public IStoryNode GetNode()
    {
        return Node;
    }

    public TransitionHookDefinition SetId(string id)
    {
        Node.Name = id;
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
        if (preexistingNode is not TransitionNode node)
            throw new InternalLogicException($"When trying to replace the hook definition's generated node with a preexisting node, the types did not match (preexisting type : {preexistingNode.GetType().Name}, generated type : {Node.GetType().Name})");

        Node = node;
    }
}
