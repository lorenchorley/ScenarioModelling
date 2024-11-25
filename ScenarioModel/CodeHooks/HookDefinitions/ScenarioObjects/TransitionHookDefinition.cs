﻿using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.References;

namespace ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;

[NodeLike<INodeHookDefinition, TransitionNode>]
public class TransitionHookDefinition(System System, string StatefulObjectName, string Transition) : INodeHookDefinition
{
    [NodeLikeProperty]
    public string? Id { get; private set; }

    public IScenarioNode GetNode()
    {
        return new TransitionNode()
        {
            // Not sure
            Name = Id ?? "",
            StatefulObject = new StatefulObjectReference(System) { Name = StatefulObjectName },
            TransitionName = Transition
        };
    }

    public TransitionHookDefinition SetId(string id)
    {
        Id = id;
        return this;
    }
}