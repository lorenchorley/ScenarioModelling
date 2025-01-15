using ScenarioModelling.Expressions.Traversal;

namespace ScenarioModelling.Expressions.SemanticTree;

public record FunctionExpression : Expression
{
    public string Name { get; set; } = null!;
    public ArgumentList Arguments { get; set; } = null!;

    public override object Accept(IExpressionVisitor visitor)
        => visitor.VisitFunction(this);
}
