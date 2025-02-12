using ScenarioModelling.CoreObjects.Expressions.Traversal;

namespace ScenarioModelling.CoreObjects.Expressions.SemanticTree;

public record ArgumentList : IExpressionNode
{
    public List<Expression> ExpressionList { get; set; } = new();

    public object Accept(IExpressionVisitor visitor)
        => visitor.VisitArgumentList(this);

    override public string ToString()
        => "ArgumentList [ " + ExpressionList.CommaSeparatedList() + " ]";
}
