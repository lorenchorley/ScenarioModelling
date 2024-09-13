using ScenarioModel.SystemObjects.Entities;

namespace ScenarioModel.SystemObjects.Constraints.Traversal;

public interface IConstraintNodeVisitor
{
    object VisitAndConstraint(AndConstraint andConstraint);
    object VisitOrConstraint(OrConstraint orConstraint);
    object VisitHasRelationConstraint(HasRelationConstraint hasRelationConstraint);

}
