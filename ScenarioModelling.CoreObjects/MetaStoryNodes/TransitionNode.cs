using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.CoreObjects.Visitors;
using ScenarioModelling.Tools.Collections.Graph;
using System.Diagnostics;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;

namespace ScenarioModelling.CoreObjects.MetaStoryNodes;

[StoryNodeLike<IStoryNode, TransitionNode>]
public record TransitionNode : StoryNode
{
    [StoryNodeLikeProperty(serialise: false)]
    public IStatefulObjectReference? StatefulObject { get; set; }

    [StoryNodeLikeProperty(serialise: false)]
    public string TransitionName { get; set; } = "";

    public TransitionNode()
    {
    }

    public override IEnumerable<SemiLinearSubGraph<IStoryNode>> TargetSubgraphs()
        => Enumerable.Empty<SemiLinearSubGraph<IStoryNode>>();

    [DebuggerNonUserCode]
    public override OneOfMetaStoryNode ToOneOf() => new(this);

    public override object Accept(IMetaStoryVisitor visitor)
        => visitor.VisitTransition(this);

    public override async Task<object> Accept(IMetaStoryAsyncVisitor visitor)
        => await visitor.VisitTransition(this);

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
