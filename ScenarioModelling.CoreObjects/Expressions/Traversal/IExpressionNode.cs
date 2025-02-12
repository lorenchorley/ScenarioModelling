namespace ScenarioModelling.CoreObjects.Expressions.Traversal;

public interface IExpressionNode
{
    object Accept(IExpressionVisitor visitor);
}
