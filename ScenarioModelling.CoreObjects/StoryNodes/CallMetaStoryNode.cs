using ProtoBuf;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.StoryNodes.BaseClasses;
using ScenarioModelling.CoreObjects.StoryNodes.Interfaces;
using ScenarioModelling.CoreObjects.Visitors;
using ScenarioModelling.Tools.Collections.Graph;
using System.Diagnostics;

namespace ScenarioModelling.CoreObjects.StoryNodes;

[ProtoContract]
[StoryNodeLike<IStoryNode, CallMetaStoryNode>]
public record CallMetaStoryNode : StoryNode, IFlowNode
{
    [ProtoMember(1)]
    [StoryNodeLikeProperty]
    public string MetaStoryName { get; set; }

    public CallMetaStoryNode()
    {
    }

    public override IEnumerable<SemiLinearSubGraph<IStoryNode>> TargetSubgraphs()
        => Enumerable.Empty<SemiLinearSubGraph<IStoryNode>>();

    [DebuggerNonUserCode]
    public override OneOfIScenaroNode ToOneOf() => new OneOfIScenaroNode(this);

    public override object Accept(IMetaStoryVisitor visitor)
        => visitor.VisitCallMetaStory(this);

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