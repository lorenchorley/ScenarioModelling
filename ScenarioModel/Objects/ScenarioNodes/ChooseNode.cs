﻿using ScenarioModel.Collections.Graph;
using ScenarioModel.Execution.Events;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Objects.ScenarioNodes.DataClasses;
using ScenarioModel.Objects.ScenarioNodes.Interfaces;
using ScenarioModel.Objects.Visitors;
using System.Diagnostics;

namespace ScenarioModel.Objects.ScenarioNodes;

// TODO A subgraph for each choice rather than relying on jump nodes
// Could act more like a switch

[NodeLike<IScenarioNode, ChooseNode>]
public record ChooseNode : ScenarioNode<ChoiceSelectedEvent>, IFlowNode
{
    [NodeLikeProperty(serialise: false)]
    public ChoiceList Choices { get; set; } = new();

    public ChooseNode()
    {
    }

    public override ChoiceSelectedEvent GenerateEvent(EventGenerationDependencies dependencies)
    {
        return new ChoiceSelectedEvent() { ProducerNode = this };
    }

    public override IEnumerable<SemiLinearSubGraph<IScenarioNode>> TargetSubgraphs()
        => Enumerable.Empty<SemiLinearSubGraph<IScenarioNode>>();

    [DebuggerNonUserCode]
    public override OneOfIScenaroNode ToOneOf() => new OneOfIScenaroNode(this);

    public override object Accept(IScenarioVisitor visitor)
        => visitor.VisitChooseNode(this);

    public override bool IsFullyEqv(IScenarioNode other)
    {
        if (other is not ChooseNode otherNode)
            return false;

        if (!otherNode.Name.IsEqv(Name))
            return false;

        if (!otherNode.Choices.IsEqv(Choices))
            return false;

        return true;
    }
}