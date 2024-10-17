namespace ScenarioModel.Expressions.Traversal;

public interface IExpressionNode
{
    object Accept(IExpressionVisitor visitor);
}
