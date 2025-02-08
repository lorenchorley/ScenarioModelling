using ProtoBuf;
using ScenarioModelling.Collections.Graph;
using ScenarioModelling.Execution.Events;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;
using ScenarioModelling.Objects.StoryNodes.DataClasses;
using ScenarioModelling.Objects.Visitors;

namespace ScenarioModelling.Objects.StoryNodes;

[ProtoContract]
[StoryNodeLike<IStoryNode, MetadataNode>]
public record MetadataNode : StoryNode<MetadataEvent>
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