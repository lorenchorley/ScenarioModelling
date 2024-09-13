using ScenarioModel.References;
using ScenarioModel.SystemObjects.Constraints.Traversal;

namespace ScenarioModel.SystemObjects.Entities;

public class HasRelationConstraint : ConstraintExpression
{
    public RelationReference Ref { get; set; } = null!;
    public IRelatableObjectReference RelatableObject { get; set; } = null!;

    public override object Accept(IConstraintNodeVisitor visitor)
        => visitor.VisitHasRelationConstraint(this);
}
