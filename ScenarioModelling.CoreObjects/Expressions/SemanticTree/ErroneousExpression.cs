using ScenarioModelling.CoreObjects.Expressions.Traversal;

namespace ScenarioModelling.CoreObjects.Expressions.SemanticTree;

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
