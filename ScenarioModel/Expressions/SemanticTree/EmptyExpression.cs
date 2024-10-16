using ScenarioModel.Expressions.Traversal;

namespace ScenarioModel.Expressions.SemanticTree;

public record EmptyExpression : Expression
{
    public override object Accept(IExpressionVisitor visitor)
        => visitor.VisitEmpty(this);
}
