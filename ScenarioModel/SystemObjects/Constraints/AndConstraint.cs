
using ScenarioModel.SystemObjects.Constraints.Traversal;

namespace ScenarioModel.SystemObjects.Entities;

public class AndConstraint : ConstraintExpression
{
    public ConstraintExpression Left { get; set; }
    public ConstraintExpression Right { get; set; }

    public override object Accept(IConstraintNodeVisitor visitor)
        => visitor.VisitAndConstraint(this);
}
