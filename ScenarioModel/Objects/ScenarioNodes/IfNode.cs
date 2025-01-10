using ScenarioModel.Collections.Graph;
using ScenarioModel.Execution.Events;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Objects.ScenarioNodes.DataClasses;
using ScenarioModel.Objects.ScenarioNodes.Interfaces;
using ScenarioModel.Objects.Visitors;
using System.Diagnostics;

namespace ScenarioModel.Objects.ScenarioNodes;

[NodeLike<IScenarioNode, IfNode>]
public record IfNode : ScenarioNode<IfBlockEvent>, IScenarioNodeWithExpression, IFlowNode
{
    [NodeLikeProperty(serialise: false)]
    public Expression Condition { get; set; } = null!;

    [NodeLikeProperty(serialise: false)]
    public string OriginalConditionText { get; set; } = "";

    [NodeLikeProperty(serialise: false)]
    public SemiLinearSubGraph<IScenarioNode> SubGraph { get; set; } = new();

    public IfNode()
    {
    }

    public override IfBlockEvent GenerateEvent(EventGenerationDependencies dependencies)
    {
        IfBlockEvent e = new IfBlockEvent() { ProducerNode = this };

        var result = Condition.Accept(dependencies.Evaluator);

        if (result is not bool shouldExecuteBlock)
        {
            throw new Exception($"If condition {Condition} did not evaluate to a boolean, this is a failure of the expression validation mecanism to not correctly determine the return type.");
        }

        e.Expression = OriginalConditionText;
        e.IfBlockRun = shouldExecuteBlock;

        return e;
    }

    public override IEnumerable<SemiLinearSubGraph<IScenarioNode>> TargetSubgraphs()
    {
        yield return SubGraph;
    }

    [DebuggerNonUserCode]
    public override OneOfIScenaroNode ToOneOf() => new OneOfIScenaroNode(this);

    public override object Accept(IScenarioVisitor visitor)
        => visitor.VisitIfNode(this);

    public override bool IsFullyEqv(IScenarioNode other)
    {
        if (other is not IfNode otherNode)
            return false;

        if (!other.Name.IsEqv(Name))
            return false;

        if (!otherNode.OriginalConditionText.IsEqv(OriginalConditionText))
            return false;

        // Should not take into account subgraph ! That would be for IsDeepEqv

        return true;
    }
}