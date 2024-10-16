using ScenarioModel.Expressions.Traversal;
using ScenarioModel.References;

namespace ScenarioModel.Expressions.SemanticTree;

public record DoesNotHaveRelationExpression : Expression
{
    public string? Name { get; set; }
    public string Left { get; set; }
    public string Right { get; set; }

    public override object Accept(IExpressionVisitor visitor)
        => visitor.VisitDoesNotHaveRelation(this);
}
