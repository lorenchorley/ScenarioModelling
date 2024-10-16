using ScenarioModel.Expressions.Traversal;

namespace ScenarioModel.Expressions.SemanticTree;

public record ErroneousExpression(string Name) : IExpressionNode
{

    public object Accept(IExpressionVisitor visitor)
        => visitor.VisitErroneousExpression(this);

    override public string ToString()
        => "<" + Name + ">";
}
