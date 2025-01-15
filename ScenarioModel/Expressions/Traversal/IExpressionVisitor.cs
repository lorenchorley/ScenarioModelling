using ScenarioModelling.Expressions.SemanticTree;

namespace ScenarioModelling.Expressions.Traversal;

public interface IExpressionVisitor
{
    object VisitAnd(AndExpression exp);
    object VisitOr(OrExpression exp);
    object VisitHasRelation(HasRelationExpression exp);
    object VisitCompositeValue(CompositeValue value);
    object VisitEmpty(EmptyExpression exp);
    object VisitDoesNotHaveRelation(DoesNotHaveRelationExpression exp);
    object VisitArgumentList(ArgumentList list);
    object VisitFunction(FunctionExpression exp);
    object VisitNotEqual(NotEqualExpression exp);
    object VisitEqual(EqualExpression exp);
    object VisitErroneousExpression(ErroneousExpression exp);
    object VisitBrackets(BracketsExpression exp);
}
