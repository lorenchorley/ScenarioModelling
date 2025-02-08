using ProtoBuf;
using ScenarioModelling.Collections.Graph;
using ScenarioModelling.Execution.Events;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;
using ScenarioModelling.Objects.StoryNodes.DataClasses;
using ScenarioModelling.Objects.Visitors;
using System.Diagnostics;

namespace ScenarioModelling.Objects.StoryNodes;

[ProtoContract]
[StoryNodeLike<IStoryNode, DialogNode>]
public record DialogNode : StoryNode<DialogEvent>
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

    public override DialogEvent GenerateEvent(EventGenerationDependencies dependencies)
    {
        DialogEvent e = new DialogEvent()
        {
            Character = Character,
            ProducerNode = this,
        };

        string text = TextTemplate;
        text = dependencies.Interpolator.ReplaceInterpolations(text);
        e.Text = text;

        return e;
    }

    public override IEnumerable<SemiLinearSubGraph<IStoryNode>> TargetSubgraphs()
        => Enumerable.Empty<SemiLinearSubGraph<IStoryNode>>();

    [DebuggerNonUserCode]
    public override OneOfIScenaroNode ToOneOf() => new OneOfIScenaroNode(this);

    public override object Accept(IMetaStoryVisitor visitor)
        => visitor.VisitDialogNode(this);

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
