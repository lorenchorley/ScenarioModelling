using ProtoBuf;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.Expressions.SemanticTree;
using ScenarioModelling.CoreObjects.StoryNodes.BaseClasses;
using ScenarioModelling.CoreObjects.StoryNodes.Interfaces;
using ScenarioModelling.CoreObjects.Visitors;
using ScenarioModelling.Tools.Collections.Graph;
using System.Diagnostics;

namespace ScenarioModelling.CoreObjects.StoryNodes;

[ProtoContract]
[StoryNodeLike<IStoryNode, IfNode>]
public record IfNode : StoryNode, IStoryNodeWithExpression, IFlowNode
{
    [ProtoMember(1)]
    [StoryNodeLikeProperty(serialise: false)]
    public Expression Condition { get; set; } = null!;

    [ProtoMember(2)]
    [StoryNodeLikeProperty(serialise: false)]
    public string OriginalConditionText { get; set; } = "";

    [ProtoMember(3)]
    [StoryNodeLikeProperty(serialise: false)]
    public SemiLinearSubGraph<IStoryNode> SubGraph { get; set; } = new();

    public IfNode()
    {
    }

    public override IEnumerable<SemiLinearSubGraph<IStoryNode>> TargetSubgraphs()
    {
        yield return SubGraph;
    }

    [DebuggerNonUserCode]
    public override OneOfIScenaroNode ToOneOf() => new OneOfIScenaroNode(this);

    public override object Accept(IMetaStoryVisitor visitor)
        => visitor.VisitIfNode(this);

    public override bool IsFullyEqv(IStoryNode other)
    {
        if (other is not IfNode otherNode)
            return false;

        if (!other.Name.IsEqv(Name))
            return false;

        if (!otherNode.OriginalConditionText.IsEqv(OriginalConditionText))
            return false;

        // Should not take into account subgraph ! That would be for IsDeepEqv

        return true;
    }
}