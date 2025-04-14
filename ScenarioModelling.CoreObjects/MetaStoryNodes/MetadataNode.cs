using ProtoBuf;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.CoreObjects.Visitors;
using ScenarioModelling.Tools.Collections.Graph;

namespace ScenarioModelling.CoreObjects.MetaStoryNodes;

[ProtoContract]
[StoryNodeLike<IStoryNode, MetadataNode>]
public record MetadataNode : StoryNode
{
    [ProtoMember(1)]
    [StoryNodeLikeProperty]
    public string Key { get; set; } = "";

    [ProtoMember(2)]
    [StoryNodeLikeProperty]
    public string MetadataType { get; set; } = "";

    [ProtoMember(3)]
    [StoryNodeLikeProperty]
    public string Value { get; set; } = "";

    public MetadataNode()
    {
    }

    public override object Accept(IMetaStoryVisitor visitor)
        => visitor.VisitMetadata(this);

    public override async Task<object> Accept(IMetaStoryAsyncVisitor visitor)
        => await visitor.VisitMetadata(this);

    public override IEnumerable<SemiLinearSubGraph<IStoryNode>> TargetSubgraphs()
        => Enumerable.Empty<SemiLinearSubGraph<IStoryNode>>();

    public override OneOfScenaroNode ToOneOf() => new OneOfScenaroNode(this);

    public override bool IsFullyEqv(IStoryNode other)
    {
        throw new NotImplementedException();
    }
}