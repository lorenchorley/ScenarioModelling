using ScenarioModelling.CoreObjects.Expressions.Traversal;

namespace ScenarioModelling.CoreObjects.Expressions.SemanticTree;

public record AndExpression : Expression
{
    public Expression Left { get; set; } = null!;
    public Expression Right { get; set; } = null!;

    public override object Accept(IExpressionVisitor visitor)
        => visitor.VisitAnd(this);
}
