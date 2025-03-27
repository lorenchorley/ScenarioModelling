using ProtoBuf;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.CoreObjects.Visitors;
using ScenarioModelling.Tools.Collections.Graph;
using System.Diagnostics;

namespace ScenarioModelling.CoreObjects.MetaStoryNodes;

[ProtoContract]
[StoryNodeLike<IStoryNode, DialogNode>]
public record DialogNode : StoryNode
{
    [ProtoMember(1)]
    [StoryNodeLikeProperty(serialisedName: "Text")]
    public string TextTemplate { get; set; } = "";

    [ProtoMember(2)]
    [StoryNodeLikeProperty(doNotSerialiseIfNullOrEmpty: true)]
    public string? Character { get; set; } = null;

    public DialogNode()
    {
    }

    public override IEnumerable<SemiLinearSubGraph<IStoryNode>> TargetSubgraphs()
        => Enumerable.Empty<SemiLinearSubGraph<IStoryNode>>();

    [DebuggerNonUserCode]
    public override OneOfScenaroNode ToOneOf() => new OneOfScenaroNode(this);

    public override object Accept(IMetaStoryVisitor visitor)
        => visitor.VisitDialog(this);

    public override bool IsFullyEqv(IStoryNode other)
    {
        if (other is not DialogNode otherNode)
            return false;

        if (!otherNode.Name.IsEqv(Name))
            return false;

        if (!otherNode.TextTemplate.IsEqv(TextTemplate))
            return false;

        if (!otherNode.Character.IsEqvCountingNulls(Character))
            return false;

        return true;
    }
}
