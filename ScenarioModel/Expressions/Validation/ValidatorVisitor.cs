using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.Expressions.Traversal;
using ScenarioModel.References;
using ScenarioModel.Validation;

namespace ScenarioModel.Expressions.Validation;

public class ValidatorVisitor : IExpressionVisitor
{
    public ValidationErrors Errors { get; } = new();

    private readonly System _system;

    public ValidatorVisitor(System system)
    {
        _system = system;
    }

    public object VisitAnd(AndExpression andConstraint)
    {
        return Errors;
    }

    public object VisitOr(OrExpression orConstraint)
    {
        return Errors;
    }

    public object VisitHasRelation(HasRelationExpression hasRelationExpression)
    {
        var reference = new RelationReference()
        {
            RelationName = hasRelationExpression.Name,
            FirstRelatableName = hasRelationExpression.Left,
            SecondRelatableName = hasRelationExpression.Right
        };

        return reference.ResolveReference(_system).Match(
            Some: relation => true,
            None: () => false
            );
    }

    public object VisitValueComposite(ValueComposite valueComposite)
    {
        throw new NotImplementedException();
    }

    public object VisitEmpty(EmptyExpression emptyExpression)
    {
        throw new NotImplementedException();
    }

    public object VisitDoesNotHaveRelation(DoesNotHaveRelationExpression doesNotHaveRelationExpression)
    {
        var reference = new RelationReference()
        {
            RelationName = doesNotHaveRelationExpression.Name,
            FirstRelatableName = doesNotHaveRelationExpression.Left,
            SecondRelatableName = doesNotHaveRelationExpression.Right
        };

        return reference.ResolveReference(_system).Match(
            Some: relation => false,
            None: () => true
            );
    }

    public object VisitArgumentList(ArgumentList argumentList)
    {
        throw new NotImplementedException();
    }

    public object VisitFunction(FunctionExpression functionExpression)
    {
        throw new NotImplementedException();
    }

    public object VisitNotEqual(NotEqualExpression notEqualExpression)
    {
        throw new NotImplementedException();
    }

    public object VisitEqual(EqualExpression equalExpression)
    {
        throw new NotImplementedException();
    }

    public object VisitErroneousExpression(ErroneousExpression erroneousExpression)
    {
        throw new NotImplementedException();
    }
}
