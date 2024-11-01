using ScenarioModel.Collections;
using ScenarioModel.Execution.Events;
using ScenarioModel.Expressions.Evaluation;
using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.Objects.ScenarioObjects.BaseClasses;
using ScenarioModel.Objects.ScenarioObjects.DataClasses;

namespace ScenarioModel.Objects.ScenarioObjects;

public record IfNode : ScenarioNode<IfBlockEvent>
{
    public string Name { get; set; } = "If";
    public Expression Condition { get; set; } = null!;
    public SemiLinearSubGraph<IScenarioNode> SubGraph { get; set; } = new();

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
}