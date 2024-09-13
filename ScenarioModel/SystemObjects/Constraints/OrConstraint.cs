using ScenarioModel.SystemObjects.Constraints.Traversal;

namespace ScenarioModel.SystemObjects.Entities;

public class OrConstraint : ConstraintExpression
{
    public ConstraintExpression Left { get; set; } = null!;
    public ConstraintExpression Right { get; set; } = null!;

    public override object Accept(IConstraintNodeVisitor visitor)
        => visitor.VisitOrConstraint(this);
}
