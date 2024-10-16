using ScenarioModel.Expressions.SemanticTree;

namespace ScenarioModel.Expressions.Traversal;

public interface IExpressionVisitor
{
    object VisitAnd(AndExpression andConstraint);
    object VisitOr(OrExpression orConstraint);
    object VisitHasRelation(HasRelationExpression hasRelationConstraint);
    object VisitValueComposite(ValueComposite valueComposite);
    object VisitEmpty(EmptyExpression emptyExpression);
    object VisitDoesNotHaveRelation(DoesNotHaveRelationExpression doesNotHaveRelationExpression);
    object VisitArgumentList(ArgumentList argumentList);
    object VisitFunction(FunctionExpression functionExpression);
    object VisitNotEqual(NotEqualExpression notEqualExpression);
    object VisitEqual(EqualExpression equalExpression);
    object VisitErroneousExpression(ErroneousExpression erroneousExpression);
}
