using ScenarioModelling.Expressions.Traversal;

namespace ScenarioModelling.Expressions.SemanticTree;

public record ErroneousExpression : IExpressionNode
{
    public string Name { get; }

    public ErroneousExpression(string name)
    {
        Name = name;
    }

    public object Accept(IExpressionVisitor visitor)
        => visitor.VisitErroneousExpression(this);

    override public string ToString()
        => "<" + Name + ">";
}
