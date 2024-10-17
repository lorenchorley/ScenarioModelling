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

    public object VisitAnd(AndExpression exp)
    {
        throw new NotImplementedException();
    }

    public object VisitOr(OrExpression exp)
    {
        throw new NotImplementedException();
    }

    public object VisitHasRelation(HasRelationExpression exp)
    {
        // Relation name must be valid in the given system even if not applied to something
        var reference = new RelationReference()
        {
            RelationName = exp.Name,
            FirstRelatableName = exp.Left,
            SecondRelatableName = exp.Right
        };

        if (reference.ResolveReference(_system).IsNone)
            Errors.Add(new ValidationError($"Relation {exp.Name} not found in system"));

        // Relatable object must exist in the system
        var firstRelatableReference = new RelatableObjectReference()
        {
            Identifier = exp.Left
        };

        if (!firstRelatableReference.ResolveReference(_system).IsNone)
            Errors.Add(new ValidationError($"Relatable object {exp.Left} not found in system"));

        var secondRelatableReference = new RelatableObjectReference()
        {
            Identifier = exp.Right
        };

        if (!secondRelatableReference.ResolveReference(_system).IsNone)
            Errors.Add(new ValidationError($"Relatable object {exp.Right} not found in system"));

        return null;
    }

    public object VisitValueComposite(ValueComposite value)
    {
        var reference = new RelatableObjectReference()
        {
            Identifier = value
        };

        if (reference.ResolveReference(_system).IsNone)
            Errors.Add(new ValidationError($"Relatable object {value} not found in system"));

        return null;
    }

    public object VisitEmpty(EmptyExpression exp)
    {
        Errors.Add(new ValidationError("Expression was empty"));

        return null;
    }

    public object VisitDoesNotHaveRelation(DoesNotHaveRelationExpression exp)
    {
        // Relation name must be valid in the given system even if not applied to something
        var reference = new RelationReference()
        {
            RelationName = exp.Name,
            FirstRelatableName = exp.Left,
            SecondRelatableName = exp.Right
        };

        if (reference.ResolveReference(_system).IsNone)
            Errors.Add(new ValidationError($"Relation {exp.Name} not found in system"));

        // Relatable object must exist in the system
        var firstRelatableReference = new RelatableObjectReference()
        {
            Identifier = exp.Left
        };

        if (!firstRelatableReference.ResolveReference(_system).IsNone)
            Errors.Add(new ValidationError($"Relatable object {exp.Left} not found in system"));

        var secondRelatableReference = new RelatableObjectReference()
        {
            Identifier = exp.Right
        };

        if (!secondRelatableReference.ResolveReference(_system).IsNone)
            Errors.Add(new ValidationError($"Relatable object {exp.Right} not found in system"));

        return null;
    }

    public object VisitArgumentList(ArgumentList list)
    {
        throw new NotImplementedException();
    }

    public object VisitFunction(FunctionExpression exp)
    {
        throw new NotImplementedException();
    }

    public object VisitNotEqual(NotEqualExpression exp)
    {
        throw new NotImplementedException();
    }

    public object VisitEqual(EqualExpression exp)
    {
        throw new NotImplementedException();
    }

    public object VisitErroneousExpression(ErroneousExpression exp)
    {
        throw new NotImplementedException();
    }

    public object VisitBrackets(BracketsExpression exp)
    {
        throw new NotImplementedException();
    }
}
