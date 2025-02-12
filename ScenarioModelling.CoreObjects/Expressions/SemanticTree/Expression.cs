using ScenarioModelling.CoreObjects.Expressions.Traversal;

namespace ScenarioModelling.CoreObjects.Expressions.SemanticTree;

public abstract record Expression : IExpressionNode
{
    public Type? Type { get; set; }
    public abstract object Accept(IExpressionVisitor visitor);
}
