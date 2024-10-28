using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.Expressions.Traversal;
using ScenarioModel.References;
using ScenarioModel.Validation;

namespace ScenarioModel.Expressions.Validation;

public class ExpressionValidator : IExpressionVisitor
{
    public ValidationErrors Errors { get; } = new();

    private readonly System _system;

    public ExpressionValidator(System system)
    {
        _system = system;
    }

    public object VisitAnd(AndExpression exp)
    {
        return this;
    }

    public object VisitOr(OrExpression exp)
    {
        return this;
    }

    public object VisitHasRelation(HasRelationExpression exp)
    {
        // Relation name must be valid in the given system even if not applied to something
        //var reference = new RelationReference()
        //{
        //    RelationName = exp.Name,
        //    FirstRelatableName = exp.Left,
        //    SecondRelatableName = exp.Right
        //};

        //if (reference.ResolveReference(_system).IsNone)
        //    Errors.Add(new ValidationError($"Relation {exp.Name} not found in system"));

        // Relatable object must exist in the system
        var firstRelatableReference = new RelatableObjectReference()
        {
            Identifier = exp.Left
        };

        if (firstRelatableReference.ResolveReference(_system).IsNone)
            Errors.Add(new ValidationError($"Relatable object {exp.Left} not found in system"));

        var secondRelatableReference = new RelatableObjectReference()
        {
            Identifier = exp.Right
        };

        if (secondRelatableReference.ResolveReference(_system).IsNone)
            Errors.Add(new ValidationError($"Relatable object {exp.Right} not found in system"));

        return this;
    }

    public object VisitDoesNotHaveRelation(DoesNotHaveRelationExpression exp)
    {
        // Relation name must be valid in the given system even if not applied to something
        //var reference = new RelationReference()
        //{
        //    RelationName = exp.Name,
        //    FirstRelatableName = exp.Left,
        //    SecondRelatableName = exp.Right
        //};

        //if (reference.ResolveReference(_system).IsNone)
        //{
        //    string name = string.IsNullOrEmpty(exp.Name)
        //        ? exp.ToString()
        //        : exp.Name;
        //    Errors.Add(new ValidationError($"Relation {name} not found in system"));
        //}

        // Relatable object must exist in the system
        var firstRelatableReference = new RelatableObjectReference()
        {
            Identifier = exp.Left
        };

        // The relatable object must exist in the system
        if (firstRelatableReference.ResolveReference(_system).IsNone)
            Errors.Add(new ValidationError($"Relatable object {exp.Left} not found in system"));

        var secondRelatableReference = new RelatableObjectReference()
        {
            Identifier = exp.Right
        };

        // The relatable object must exist in the system
        if (secondRelatableReference.ResolveReference(_system).IsNone)
            Errors.Add(new ValidationError($"Relatable object {exp.Right} not found in system"));

        return this;
    }

    public object VisitValueComposite(ValueComposite value)
    {
        var reference = new RelatableObjectReference()
        {
            Identifier = value
        };

        if (reference.ResolveReference(_system).IsNone && value.ValueList.Count > 1)
        {
            Errors.Add(new ValidationError($"Relatable object {value} not found in system"));
        }

        return this;
    }

    public object VisitEmpty(EmptyExpression exp)
    {
        Errors.Add(new ValidationError("Expression was empty"));

        return this;
    }

    public object VisitArgumentList(ArgumentList list)
    {
        return this;
    }

    public object VisitFunction(FunctionExpression exp)
    {
        return this;
    }

    public object VisitNotEqual(NotEqualExpression exp)
    {
        return this;
    }

    public object VisitEqual(EqualExpression exp)
    {
        return this;
    }

    public object VisitErroneousExpression(ErroneousExpression exp)
    {
        return this;
    }

    public object VisitBrackets(BracketsExpression exp)
    {
        return this;
    }
}
