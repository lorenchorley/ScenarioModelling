using ScenarioModelling.Expressions.Traversal;

namespace ScenarioModelling.Expressions.SemanticTree;

public abstract record Expression : IExpressionNode
{
    public Type? Type { get; set; }
    public abstract object Accept(IExpressionVisitor visitor);
}
