using ScenarioModel.Expressions.Traversal;

namespace ScenarioModel.Expressions.SemanticTree;

public abstract class Expression : IExpressionNode
{
    public abstract object Accept(IExpressionVisitor visitor);
}
