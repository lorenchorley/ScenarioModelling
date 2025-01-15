using ScenarioModelling.Collections.Graph;
using ScenarioModelling.Execution.Events;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.ScenarioNodes.BaseClasses;
using ScenarioModelling.Objects.ScenarioNodes.DataClasses;
using ScenarioModelling.Objects.ScenarioNodes.Interfaces;
using ScenarioModelling.Objects.Visitors;
using System.Diagnostics;

namespace ScenarioModelling.Objects.ScenarioNodes;

[NodeLike<IScenarioNode, JumpNode>]
public record JumpNode : ScenarioNode<JumpEvent>, IFlowNode
{
    [NodeLikeProperty]
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

    public override IEnumerable<SemiLinearSubGraph<IScenarioNode>> TargetSubgraphs()
        => Enumerable.Empty<SemiLinearSubGraph<IScenarioNode>>();

    [DebuggerNonUserCode]
    public override OneOfIScenaroNode ToOneOf() => new OneOfIScenaroNode(this);

    public override object Accept(IScenarioVisitor visitor)
        => visitor.VisitJumpNode(this);

    public override bool IsFullyEqv(IScenarioNode other)
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
