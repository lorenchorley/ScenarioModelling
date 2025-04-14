using ProtoBuf;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.CoreObjects.MetaStoryNodes.DataClasses;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;
using ScenarioModelling.CoreObjects.Visitors;
using ScenarioModelling.Tools.Collections.Graph;
using System.Diagnostics;

namespace ScenarioModelling.CoreObjects.MetaStoryNodes;

// TODO A subgraph for each choice rather than relying on jump nodes
// Could act more like a switch

[ProtoContract]
[StoryNodeLike<IStoryNode, ChooseNode>]
public record ChooseNode : StoryNode, IFlowNode
{
    [ProtoMember(1)]
    [StoryNodeLikeProperty(serialise: false)]
    public ChoiceList Choices { get; set; } = new();

    public ChooseNode()
    {
    }

    public override IEnumerable<SemiLinearSubGraph<IStoryNode>> TargetSubgraphs()
        => Enumerable.Empty<SemiLinearSubGraph<IStoryNode>>();

    [DebuggerNonUserCode]
    public override OneOfScenaroNode ToOneOf() => new OneOfScenaroNode(this);

    public override object Accept(IMetaStoryVisitor visitor)
        => visitor.VisitChoose(this);

    public override async Task<object> Accept(IMetaStoryAsyncVisitor visitor)
        => await visitor.VisitChoose(this);

    public override bool IsFullyEqv(IStoryNode other)
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