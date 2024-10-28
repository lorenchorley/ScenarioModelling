using ScenarioModel;
using ScenarioModel.Execution.Events;
using ScenarioModel.Expressions.Evaluation;
using ScenarioModel.Objects.Scenario;

namespace ScenarioModelling_ConsoleFront.NodeHandlers;

internal class IfNodeHandler : NodeHandler<IfNode, IfBlockEvent>
{
    public override void Handle(IfNode node, IfBlockEvent e)
    {
        ExpressionEvalator visitor = new(Context.System);

        var result = node.Expression.Accept(visitor);

        if (result is not bool shouldExecuteBlock)
        {
            throw new Exception($"If condition {node.Expression} did not evaluate to a boolean, this is a failure of the expression validation mecanism to not correctly determine the return type.");
        }

        e.IfBlockRun = shouldExecuteBlock;

        // TODO How to start the if block?
        // Should be managed by DialogFactory by checking the last event
        throw new NotImplementedException();
    }
}
