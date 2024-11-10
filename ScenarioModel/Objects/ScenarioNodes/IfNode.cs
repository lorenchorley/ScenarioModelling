using ScenarioModel.Collections;
using ScenarioModel.Execution.Events;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Objects.ScenarioNodes.DataClasses;

namespace ScenarioModel.Objects.ScenarioNodes;

[NodeLike<IScenarioNode, IfNode>]
public record IfNode : ScenarioNode<IfBlockEvent>
{
    [NodeLikeProperty(serialise: false)]
    public Expression Condition { get; set; } = null!;

    [NodeLikeProperty(serialise: false)]
    public SemiLinearSubGraph<IScenarioNode> SubGraph { get; set; } = new();

    public IfNode()
    {
        Name = "If";
    }

    public override IfBlockEvent GenerateEvent(EventGenerationDependencies dependencies)
    {
        IfBlockEvent e = new IfBlockEvent() { ProducerNode = this };

        var result = Condition.Accept(dependencies.Evaluator);

        if (result is not bool shouldExecuteBlock)
        {
            throw new Exception($"If condition {Condition} did not evaluate to a boolean, this is a failure of the expression validation mecanism to not correctly determine the return type.");
        }

        e.IfBlockRun = shouldExecuteBlock;

        return e;
    }

    public override IEnumerable<SemiLinearSubGraph<IScenarioNode>> TargetSubgraphs()
    {
        yield return SubGraph;
    }

    public override OneOfIScenaroNode ToOneOf() => new OneOfIScenaroNode(this);
}