using ProtoBuf;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.Expressions.SemanticTree;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.CoreObjects.Visitors;
using ScenarioModelling.Tools.Collections.Graph;
using System.Diagnostics;

namespace ScenarioModelling.CoreObjects.MetaStoryNodes;

[ProtoContract]
[StoryNodeLike<IStoryNode, AssertNode>]
public record AssertNode : StoryNode, IStoryNodeWithExpression
{
    [ProtoMember(1)]
    [StoryNodeLikeProperty(serialise: false)]
    public Expression AssertionExpression { get; set; } = null!;

    [ProtoMember(2)]
    [StoryNodeLikeProperty(serialise: false)]
    public string OriginalExpressionText { get; set; } = "";

    public AssertNode()
    {
    }

    public override IEnumerable<SemiLinearSubGraph<IStoryNode>> TargetSubgraphs()
        => Enumerable.Empty<SemiLinearSubGraph<IStoryNode>>();

    [DebuggerNonUserCode]
    public override OneOfScenaroNode ToOneOf() => new OneOfScenaroNode(this);

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
