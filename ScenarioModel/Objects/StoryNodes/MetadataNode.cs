using ScenarioModelling.Collections.Graph;
using ScenarioModelling.Execution.Events;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;
using ScenarioModelling.Objects.StoryNodes.DataClasses;
using ScenarioModelling.Objects.Visitors;

namespace ScenarioModelling.Objects.StoryNodes;

[StoryNodeLike<IStoryNode, MetadataNode>]
public record MetadataNode : StoryNode<MetadataEvent>
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
        => visitor.VisitMetadataNode(this);

    public override MetadataEvent GenerateEvent(EventGenerationDependencies dependencies)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<SemiLinearSubGraph<IStoryNode>> TargetSubgraphs()
        => Enumerable.Empty<SemiLinearSubGraph<IStoryNode>>();

    public override OneOfIScenaroNode ToOneOf() => new OneOfIScenaroNode(this);

    public override bool IsFullyEqv(IStoryNode other)
    {
        throw new NotImplementedException();
    }
}