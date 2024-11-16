using ScenarioModel.Collections;
using ScenarioModel.Execution.Events;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Objects.ScenarioNodes.DataClasses;

namespace ScenarioModel.Objects.ScenarioNodes;

[NodeLike<IScenarioNode, WhileNode>]
public record WhileNode : ScenarioNode<WhileLoopConditionCheckEvent>
{
    [NodeLikeProperty(serialise: false)]
    public Expression Condition { get; set; } = null!;

    [NodeLikeProperty(serialise: false)]
    public SemiLinearSubGraph<IScenarioNode> SubGraph { get; set; } = new();

    public WhileNode()
    {
        Name = "While";
    }

    public override WhileLoopConditionCheckEvent GenerateEvent(EventGenerationDependencies dependencies)
    {
        WhileLoopConditionCheckEvent e = new()
        {
            ProducerNode = this,
        };

        var result = Condition.Accept(dependencies.Evaluator);

        if (result is not bool shouldExecuteBlock)
        {
            throw new Exception($"While loop condition {Condition} did not evaluate to a boolean, this is a failure of the expression validation mecanism to not correctly determine the return type.");
        }

        e.LoopBlockRun = shouldExecuteBlock;

        return e;
    }

    public override IEnumerable<SemiLinearSubGraph<IScenarioNode>> TargetSubgraphs()
    {
        yield return SubGraph;
    }

    public override OneOfIScenaroNode ToOneOf() => new OneOfIScenaroNode(this);
}
