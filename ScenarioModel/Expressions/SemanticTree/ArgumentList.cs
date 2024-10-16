using ScenarioModel.Expressions.Traversal;

namespace ScenarioModel.Expressions.SemanticTree;

public record ArgumentList : IExpressionNode
{
    public List<Expression> ExpressionList { get; set; } = new();

    public object Accept(IExpressionVisitor visitor)
        => visitor.VisitArgumentList(this);

    override public string ToString()
        => "ArgumentList [ " + string.Join(", ", ExpressionList) + " ]";
}
