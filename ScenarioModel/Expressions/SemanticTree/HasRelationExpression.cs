using ScenarioModel.Expressions.Traversal;

namespace ScenarioModel.Expressions.SemanticTree;

public record HasRelationExpression : Expression
{
    public string? Name { get; set; }
    public string Left { get; set; }
    public string Right { get; set; }

    public override object Accept(IExpressionVisitor visitor)
        => visitor.VisitHasRelation(this);
}
