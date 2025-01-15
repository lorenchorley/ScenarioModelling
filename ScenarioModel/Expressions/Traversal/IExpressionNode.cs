namespace ScenarioModelling.Expressions.Traversal;

public interface IExpressionNode
{
    object Accept(IExpressionVisitor visitor);
}
