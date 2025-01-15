using ScenarioModelling.Expressions.Traversal;

namespace ScenarioModelling.Expressions.SemanticTree;

public record EmptyExpression : Expression
{
    public override object Accept(IExpressionVisitor visitor)
        => visitor.VisitEmpty(this);
}
