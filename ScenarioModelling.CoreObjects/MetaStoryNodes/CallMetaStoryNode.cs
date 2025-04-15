using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;
using ScenarioModelling.CoreObjects.Visitors;
using ScenarioModelling.Tools.Collections.Graph;
using System.Diagnostics;

namespace ScenarioModelling.CoreObjects.MetaStoryNodes;

[StoryNodeLike<IStoryNode, CallMetaStoryNode>]
public record CallMetaStoryNode : StoryNode, IFlowNode
{
    [StoryNodeLikeProperty]
    public string MetaStoryName { get; set; }

    public CallMetaStoryNode()
    {
    }

    public override IEnumerable<SemiLinearSubGraph<IStoryNode>> TargetSubgraphs()
        => Enumerable.Empty<SemiLinearSubGraph<IStoryNode>>();

    [DebuggerNonUserCode]
    public override OneOfMetaStoryNode ToOneOf() => new(this);

    public override object Accept(IMetaStoryVisitor visitor)
        => visitor.VisitCallMetaStory(this);

    public override async Task<object> Accept(IMetaStoryAsyncVisitor visitor)
        => await visitor.VisitCallMetaStory(this);

    public override bool IsFullyEqv(IStoryNode other)
    {
        if (other is not CallMetaStoryNode otherNode)
            return false;

        if (!otherNode.Name.IsEqv(Name))
            return false;

        if (!otherNode.MetaStoryName.IsEqv(MetaStoryName))
            return false;

        return true;
    }
}