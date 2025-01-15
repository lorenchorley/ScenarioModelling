using ScenarioModelling.Expressions.Traversal;

namespace ScenarioModelling.Expressions.SemanticTree;

public record ErroneousExpression(string Name) : IExpressionNode
{

    public object Accept(IExpressionVisitor visitor)
        => visitor.VisitErroneousExpression(this);

    override public string ToString()
        => "<" + Name + ">";
}
