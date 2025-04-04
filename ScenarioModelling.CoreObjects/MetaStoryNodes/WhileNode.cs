﻿using ProtoBuf;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.Expressions.SemanticTree;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;
using ScenarioModelling.CoreObjects.Visitors;
using ScenarioModelling.Tools.Collections.Graph;
using System.Diagnostics;

namespace ScenarioModelling.CoreObjects.MetaStoryNodes;

[ProtoContract]
[StoryNodeLike<IStoryNode, WhileNode>]
public record WhileNode : StoryNode, IStoryNodeWithExpression, IFlowNode
{
    [ProtoMember(1)]
    [StoryNodeLikeProperty(serialise: false)]
    public Expression AssertionExpression { get; set; } = null!;

    [ProtoMember(2)]
    [StoryNodeLikeProperty(serialise: false)]
    public string OriginalConditionText { get; set; } = "";

    [ProtoMember(3)]
    [StoryNodeLikeProperty(serialise: false)]
    public SemiLinearSubGraph<IStoryNode> SubGraph { get; set; } = new();

    public WhileNode()
    {
    }

    public override IEnumerable<SemiLinearSubGraph<IStoryNode>> TargetSubgraphs()
    {
        yield return SubGraph;
    }

    [DebuggerNonUserCode]
    public override OneOfScenaroNode ToOneOf() => new OneOfScenaroNode(this);

    public override object Accept(IMetaStoryVisitor visitor)
        => visitor.VisitWhile(this);

    public override bool IsFullyEqv(IStoryNode other)
    {
        if (other is not WhileNode otherNode)
            return false;

        if (!other.Name.IsEqv(Name))
            return false;

        if (!otherNode.OriginalConditionText.IsEqv(OriginalConditionText))
            return false;

        // Should not take into account subgraph ! That would be for IsDeepEqv

        return true;
    }
}
