using ScenarioModel.Expressions.Traversal;

namespace ScenarioModel.Expressions.SemanticTree;

public record BracketsExpression : Expression
{
    public Expression Expression { get; set; } = null!;

    public override object Accept(IExpressionVisitor visitor)
        => visitor.VisitBrackets(this);
}
