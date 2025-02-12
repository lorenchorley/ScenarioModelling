using ScenarioModelling.CoreObjects.Expressions.Traversal;

namespace ScenarioModelling.CoreObjects.Expressions.SemanticTree;

public record EmptyExpression : Expression
{
    public override object Accept(IExpressionVisitor visitor)
        => visitor.VisitEmpty(this);
}
