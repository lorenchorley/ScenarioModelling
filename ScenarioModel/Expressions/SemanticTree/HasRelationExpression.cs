using ScenarioModel.Expressions.Traversal;
using ScenarioModel.References;

namespace ScenarioModel.Expressions.SemanticTree;

public class HasRelationExpression : Expression
{
    public RelationReference Ref { get; set; } = null!;
    public IRelatableObjectReference RelatableObject { get; set; } = null!;

    public override object Accept(IExpressionVisitor visitor)
        => visitor.VisitHasRelationConstraint(this);
}
