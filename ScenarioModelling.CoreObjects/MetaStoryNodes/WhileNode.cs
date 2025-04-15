using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.Expressions.SemanticTree;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;
using ScenarioModelling.CoreObjects.Visitors;
using ScenarioModelling.Tools.Collections.Graph;
using System.Diagnostics;

namespace ScenarioModelling.CoreObjects.MetaStoryNodes;

[StoryNodeLike<IStoryNode, WhileNode>]
public record WhileNode : StoryNode, IStoryNodeWithExpression, IFlowNode
{
    [StoryNodeLikeProperty(serialise: false)]
    public Expression AssertionExpression { get; set; } = null!;

    [StoryNodeLikeProperty(serialise: false)]
    public string OriginalConditionText { get; set; } = "";

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
    public override OneOfMetaStoryNode ToOneOf() => new(this);

    public override object Accept(IMetaStoryVisitor visitor)
        => visitor.VisitWhile(this);

    public override async Task<object> Accept(IMetaStoryAsyncVisitor visitor)
        => await visitor.VisitWhile(this);

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
