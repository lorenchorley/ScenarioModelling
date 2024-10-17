using ScenarioModel.Expressions.Traversal;

namespace ScenarioModel.Expressions.SemanticTree;

public record DoesNotHaveRelationExpression : Expression
{
    public string? Name { get; set; }
    public ValueComposite Left { get; set; } = null!;
    public ValueComposite Right { get; set; } = null!;

    public override object Accept(IExpressionVisitor visitor)
        => visitor.VisitDoesNotHaveRelation(this);
}
