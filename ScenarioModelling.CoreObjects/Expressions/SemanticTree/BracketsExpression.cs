using ScenarioModelling.CoreObjects.Expressions.Traversal;

namespace ScenarioModelling.CoreObjects.Expressions.SemanticTree;

public record BracketsExpression : Expression
{
    public Expression Expression { get; set; } = null!;

    public override object Accept(IExpressionVisitor visitor)
        => visitor.VisitBrackets(this);
}
