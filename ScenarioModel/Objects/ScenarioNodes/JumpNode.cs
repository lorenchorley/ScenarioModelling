using ScenarioModel.Collections.Graph;
using ScenarioModel.Execution.Events;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Objects.ScenarioNodes.DataClasses;
using ScenarioModel.Objects.ScenarioNodes.Interfaces;
using ScenarioModel.Objects.Visitors;
using System.Diagnostics;

namespace ScenarioModel.Objects.ScenarioNodes;

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
