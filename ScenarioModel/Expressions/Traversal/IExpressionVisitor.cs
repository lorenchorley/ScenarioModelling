using ScenarioModel.Expressions.SemanticTree;

namespace ScenarioModel.Expressions.Traversal;

public interface IExpressionVisitor
{
    object VisitAnd(AndExpression exp);
    object VisitOr(OrExpression exp);
    object VisitHasRelation(HasRelationExpression exp);
    object VisitValueComposite(ValueComposite value);
    object VisitEmpty(EmptyExpression exp);
    object VisitDoesNotHaveRelation(DoesNotHaveRelationExpression exp);
    object VisitArgumentList(ArgumentList list);
    object VisitFunction(FunctionExpression exp);
    object VisitNotEqual(NotEqualExpression exp);
    object VisitEqual(EqualExpression exp);
    object VisitErroneousExpression(ErroneousExpression exp);
    object VisitBrackets(BracketsExpression exp);
}
