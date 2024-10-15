using ScenarioModel.Expressions.Traversal;

namespace ScenarioModel.Expressions.SemanticTree;

public class AndExpression : Expression
{
    public Expression Left { get; set; }
    public Expression Right { get; set; }

    public override object Accept(IExpressionVisitor visitor)
        => visitor.VisitAndConstraint(this);
}
