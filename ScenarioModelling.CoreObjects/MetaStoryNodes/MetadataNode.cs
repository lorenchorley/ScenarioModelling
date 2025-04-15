using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;
using ScenarioModelling.CoreObjects.Visitors;
using ScenarioModelling.Tools.Collections.Graph;

namespace ScenarioModelling.CoreObjects.MetaStoryNodes;

[StoryNodeLike<IStoryNode, MetadataNode>]
public record MetadataNode : StoryNode
{
    [StoryNodeLikeProperty]
    public string Key { get; set; } = "";

    [StoryNodeLikeProperty]
    public string MetadataType { get; set; } = "";

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

    public override OneOfMetaStoryNode ToOneOf() => new(this);

    public override bool IsFullyEqv(IStoryNode other)
    {
        throw new NotImplementedException();
    }
}