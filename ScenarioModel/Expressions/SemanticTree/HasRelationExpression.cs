using ScenarioModel.Expressions.Traversal;

namespace ScenarioModel.Expressions.SemanticTree;

public record HasRelationExpression : Expression
{
    public string? Name { get; set; }
    public ValueComposite Left { get; set; } = null!;
    public ValueComposite Right { get; set; } = null!;

    public override object Accept(IExpressionVisitor visitor)
        => visitor.VisitHasRelation(this);
}
