using ProtoBuf;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.CoreObjects.Visitors;
using ScenarioModelling.Tools.Collections.Graph;
using System.Diagnostics;

namespace ScenarioModelling.CoreObjects.MetaStoryNodes;

[ProtoContract]
[StoryNodeLike<IStoryNode, TransitionNode>]
public record TransitionNode : StoryNode
{
    [ProtoMember(1)]
    [StoryNodeLikeProperty(serialise: false)]
    public IStatefulObjectReference? StatefulObject { get; set; }

    [ProtoMember(2)]
    [StoryNodeLikeProperty(serialise: false)]
    public string TransitionName { get; set; } = "";

    public TransitionNode()
    {
    }

    public override IEnumerable<SemiLinearSubGraph<IStoryNode>> TargetSubgraphs()
        => Enumerable.Empty<SemiLinearSubGraph<IStoryNode>>();

    [DebuggerNonUserCode]
    public override OneOfIScenaroNode ToOneOf() => new OneOfIScenaroNode(this);

    public override object Accept(IMetaStoryVisitor visitor)
        => visitor.VisitTransition(this);

    public override bool IsFullyEqv(IStoryNode other)
    {
        if (other is not TransitionNode otherNode)
            return false;

        if (!other.Name.IsEqv(Name))
            return false;

        if (!otherNode.TransitionName.IsEqv(TransitionName))
            return false;

        ArgumentNullExceptionStandard.ThrowIfNull(StatefulObject);
        ArgumentNullExceptionStandard.ThrowIfNull(otherNode.StatefulObject);

        if (!otherNode.StatefulObject.IsEqv(StatefulObject))
            return false;

        return true;
    }
}
