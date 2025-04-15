using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.Expressions.SemanticTree;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;
using ScenarioModelling.CoreObjects.Visitors;
using ScenarioModelling.Tools.Collections.Graph;
using System.Diagnostics;

namespace ScenarioModelling.CoreObjects.MetaStoryNodes;

[StoryNodeLike<IStoryNode, AssertNode>]
public record AssertNode : StoryNode, IStoryNodeWithExpression
{
    [StoryNodeLikeProperty(serialise: false)]
    public Expression AssertionExpression { get; set; } = null!;

    [StoryNodeLikeProperty(serialise: false)]
    public string OriginalExpressionText { get; set; } = "";

    public AssertNode()
    {
    }

    public override IEnumerable<SemiLinearSubGraph<IStoryNode>> TargetSubgraphs()
        => Enumerable.Empty<SemiLinearSubGraph<IStoryNode>>();

    [DebuggerNonUserCode]
    public override OneOfMetaStoryNode ToOneOf() => new(this);

    public override object Accept(IMetaStoryVisitor visitor)
        => visitor.VisitAssert(this);
    
    public override async Task<object> Accept(IMetaStoryAsyncVisitor visitor)
        => await visitor.VisitAssert(this);

    public override bool IsFullyEqv(IStoryNode other)
    {
        if (other is not AssertNode otherNode)
            return false;

        if (!otherNode.Name.IsEqv(Name))
            return false;

        if (!otherNode.OriginalExpressionText.IsEqv(OriginalExpressionText))
            return false;

        return true;
    }
}
