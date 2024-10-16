using ScenarioModel.Expressions.Traversal;

namespace ScenarioModel.Expressions.SemanticTree;

public abstract record Expression : IExpressionNode
{
    public abstract object Accept(IExpressionVisitor visitor);
}
