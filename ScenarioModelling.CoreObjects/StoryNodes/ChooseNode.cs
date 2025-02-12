using ProtoBuf;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.StoryNodes.BaseClasses;
using ScenarioModelling.CoreObjects.StoryNodes.DataClasses;
using ScenarioModelling.CoreObjects.StoryNodes.Interfaces;
using ScenarioModelling.CoreObjects.Visitors;
using ScenarioModelling.Tools.Collections.Graph;
using System.Diagnostics;

namespace ScenarioModelling.CoreObjects.StoryNodes;

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
    public override OneOfIScenaroNode ToOneOf() => new OneOfIScenaroNode(this);

    public override object Accept(IMetaStoryVisitor visitor)
        => visitor.VisitChooseNode(this);

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