﻿using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;
using ScenarioModelling.CoreObjects.Visitors;
using ScenarioModelling.Tools.Collections.Graph;
using System.Diagnostics;

namespace ScenarioModelling.CoreObjects.MetaStoryNodes;

[StoryNodeLike<IStoryNode, JumpNode>]
public record JumpNode : StoryNode, IFlowNode
{
    [StoryNodeLikeProperty]
    public string Target { get; set; } = "";

    public JumpNode()
    {
    }

    public override IEnumerable<SemiLinearSubGraph<IStoryNode>> TargetSubgraphs()
        => Enumerable.Empty<SemiLinearSubGraph<IStoryNode>>();

    [DebuggerNonUserCode]
    public override OneOfMetaStoryNode ToOneOf() => new(this);

    public override object Accept(IMetaStoryVisitor visitor)
        => visitor.VisitJump(this);

    public override async Task<object> Accept(IMetaStoryAsyncVisitor visitor)
        => await visitor.VisitJump(this);

    public override bool IsFullyEqv(IStoryNode other)
    {
        if (other is not JumpNode otherNode)
            return false;

        if (!other.Name.IsEqv(Name))
            return false;

        if (!otherNode.Target.IsEqv(Target))
            return false;

        return true;
    }
}
