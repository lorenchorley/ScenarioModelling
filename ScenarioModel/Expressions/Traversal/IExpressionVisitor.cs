using ScenarioModel.Expressions.SemanticTree;

namespace ScenarioModel.Expressions.Traversal;

public interface IExpressionVisitor
{
    object VisitAndConstraint(AndExpression andConstraint);
    object VisitOrConstraint(OrExpression orConstraint);
    object VisitHasRelationConstraint(HasRelationExpression hasRelationConstraint);
    object VisitValueComposite(ValueComposite valueComposite);
}
