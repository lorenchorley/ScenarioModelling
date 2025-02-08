using ProtoBuf;
using ScenarioModelling.Collections.Graph;
using ScenarioModelling.Execution.Events;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;
using ScenarioModelling.Objects.StoryNodes.DataClasses;
using ScenarioModelling.Objects.StoryNodes.Interfaces;
using ScenarioModelling.Objects.Visitors;
using System.Diagnostics;

namespace ScenarioModelling.Objects.StoryNodes;

[ProtoContract]
[StoryNodeLike<IStoryNode, JumpNode>]
public record JumpNode : StoryNode<JumpEvent>, IFlowNode
{
    [ProtoMember(1)]
    [StoryNodeLikeProperty]
    public string Target { get; set; } = "";

    public JumpNode()
    {
    }

    public override JumpEvent GenerateEvent(EventGenerationDependencies dependencies)
    {
        return new JumpEvent()
        {
            Target = Target,
            ProducerNode = this,
        };
    }

    public override IEnumerable<SemiLinearSubGraph<IStoryNode>> TargetSubgraphs()
        => Enumerable.Empty<SemiLinearSubGraph<IStoryNode>>();

    [DebuggerNonUserCode]
    public override OneOfIScenaroNode ToOneOf() => new OneOfIScenaroNode(this);

    public override object Accept(IMetaStoryVisitor visitor)
        => visitor.VisitJumpNode(this);

    public override bool IsFullyEqv(IStoryNode other)
    {
        if (other is not JumpNode otherNode)
            return false;

        if (!other.Name.IsEqv(Name))
            return false;

        if (!otherNode.Target.IsEqv(Target))
            return false;

        return true;
    }
}
