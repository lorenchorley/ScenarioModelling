using ScenarioModel.Expressions.Evaluation;
using ScenarioModel.Expressions.Traversal;

namespace ScenarioModel.Expressions.SemanticTree;

public abstract record Expression : IExpressionNode
{
    public ExpressionValueType Type { get; set; } = ExpressionValueType.Unknown;
    public abstract object Accept(IExpressionVisitor visitor);
}
