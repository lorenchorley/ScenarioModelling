using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.Expressions.Common;
using ScenarioModelling.CoreObjects.Expressions.SemanticTree;
using ScenarioModelling.CoreObjects.Expressions.Traversal;
using ScenarioModelling.CoreObjects.References.GeneralisedReferences;
using ScenarioModelling.CoreObjects.SystemObjects;

namespace ScenarioModelling.CoreObjects.Expressions.Initialisation;

public class ExpressionInitialiser : IExpressionVisitor
{
    public ValidationErrors Errors { get; } = new();

    private readonly MetaState _system;

    public ExpressionInitialiser(MetaState system)
    {
        _system = system;
    }

    public object VisitAnd(AndExpression exp)
    {
        exp.Type = typeof(bool);

        exp.Left.Accept(this);
        if (exp.Left.Type != typeof(bool))
            Errors.Add(new ValidationError($"Left expression type in And Expression is not boolean : {exp.Left.Type}"));

        exp.Right.Accept(this);
        if (exp.Right.Type != typeof(bool))
            Errors.Add(new ValidationError($"Right expression type in And Expression is not boolean : {exp.Right.Type}"));

        return this;
    }

    public object VisitOr(OrExpression exp)
    {
        exp.Type = typeof(bool);

        exp.Left.Accept(this);
        if (exp.Left.Type != typeof(bool))
            Errors.Add(new ValidationError($"Left expression type in Or Expression is not boolean : {exp.Left.Type}"));

        exp.Right.Accept(this);
        if (exp.Right.Type != typeof(bool))
            Errors.Add(new ValidationError($"Right expression type in Or Expression is not boolean : {exp.Right.Type}"));

        return this;
    }

    public object VisitHasRelation(HasRelationExpression exp)
    {
        exp.Type = typeof(bool);

        // Relation name must be valid in the given system even if not applied to something
        //var reference = new RelationReference()
        //{
        //    RelationName = exp.Name,
        //    FirstRelatableName = exp.Left,
        //    SecondRelatableName = exp.Right
        //};

        //if (reference.ResolveReference(_system).IsNone)
        //    Errors.Add(new ValidationError($"Relation {exp.Name} not found in system"));

        exp.Left.Accept(this);
        if (!IsRelatableType(exp.Left.Type))
            Errors.Add(new ValidationError($"Object type (Left) in Has Relation Expression is not a relatable object type : {exp.Left.Type}"));

        // Relatable object must exist in the system
        var firstRelatableReference = new CompositeValueObjectReference(_system)
        {
            Identifier = exp.Left
        };

        if (firstRelatableReference.ResolveReference().IsNone)
            Errors.Add(new ValidationError($"Relatable object {exp.Left} not found in system"));


        exp.Right.Accept(this);
        if (!IsRelatableType(exp.Right.Type))
            Errors.Add(new ValidationError($"Relation name (Right) in Has Relation Expression is not a relatable object type : {exp.Right.Type}"));

        var secondRelatableReference = new CompositeValueObjectReference(_system)
        {
            Identifier = exp.Right
        };

        // Reference object must exist in the system
        if (secondRelatableReference.ResolveReference().IsNone)
            Errors.Add(new ValidationError($"Relatable object {exp.Right} not found in system"));

        return this;
    }

    private static bool IsRelatableType(Type type)
        => type == typeof(Entity) || type == typeof(Aspect);

    public object VisitDoesNotHaveRelation(DoesNotHaveRelationExpression exp)
    {
        exp.Type = typeof(bool);

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

        exp.Left.Accept(this);
        if (!IsRelatableType(exp.Left.Type))
            Errors.Add(new ValidationError($"Object type (Left) in Does Not Have Relation Expression is not a relatable object type : {exp.Left.Type}"));

        // Relatable object must exist in the system
        var firstRelatableReference = new CompositeValueObjectReference(_system)
        {
            Identifier = exp.Left
        };

        // The relatable object must exist in the system
        if (firstRelatableReference.ResolveReference().IsNone)
            Errors.Add(new ValidationError($"Relatable object {exp.Left} not found in system"));


        exp.Right.Accept(this);
        if (!IsRelatableType(exp.Right.Type))
            Errors.Add(new ValidationError($"Relation name (Right) in Does Not Have Relation Expression is not a relatable object type : {exp.Right.Type}"));

        var secondRelatableReference = new CompositeValueObjectReference(_system)
        {
            Identifier = exp.Right
        };

        // The relatable object must exist in the system
        if (secondRelatableReference.ResolveReference().IsNone)
            Errors.Add(new ValidationError($"Relatable object {exp.Right} not found in system"));

        return this;
    }

    public object VisitCompositeValue(CompositeValue value)
    {
        var reference = new CompositeValueObjectReference(_system)
        {
            Identifier = value
        };

        var referencedValue = reference.ResolveReference();
        if (referencedValue.IsNone)
        {
            if (value.ValueList.Count > 1)
            {
                value.Type = null;
                Errors.Add(new ValidationError($"Relatable object '{value}' not found in system"));
                return this;
            }

            string stringValue = value.ValueList[0];

            if (stringValue.IsEqv("true") || stringValue.IsEqv("false"))
            {
                value.Type = typeof(bool);
                return this;
            }
            else
            {
                // String
                value.Type = typeof(string);

                return this;
            }
        }

        if (referencedValue.Case is Entity)
        {
            value.Type = typeof(Entity);
        }
        else if (referencedValue.Case is Aspect)
        {
            value.Type = typeof(Aspect);
        }
        else if (referencedValue.Case is Relation)
        {
            value.Type = typeof(Relation);
        }
        else if (referencedValue.Case is State)
        {
            value.Type = typeof(State);
        }
        else
        {
            value.Type = null;
            Errors.Add(new ValidationError($"Relatable object '{value}' found but of unknown type '{referencedValue.Case.GetType().Name}'"));
        }

        return this;
    }

    public object VisitEmpty(EmptyExpression exp)
    {
        exp.Type = null;

        Errors.Add(new ValidationError("Expression was empty"));

        return this;
    }

    public object VisitArgumentList(ArgumentList list)
    {
        return this;
    }

    public object VisitFunction(FunctionExpression exp)
    {
        exp.Type = null;
        return this;
    }

    public object VisitNotEqual(NotEqualExpression exp)
    {
        exp.Type = typeof(bool);

        exp.Left.Accept(this);
        exp.Right.Accept(this);

        EqualityFunctions.CheckEqualityTypeCases(exp.Left.Type, exp.Right.Type, _system);

        return this;
    }

    public object VisitEqual(EqualExpression exp)
    {
        exp.Type = typeof(bool); // Should Type be a OneOf<Unknown, Type> ?

        exp.Left.Accept(this);
        exp.Right.Accept(this);

        EqualityFunctions.CheckEqualityTypeCases(exp.Left.Type, exp.Right.Type, _system);

        return this;
    }

    public object VisitErroneousExpression(ErroneousExpression exp)
    {
        return this;
    }

    public object VisitBrackets(BracketsExpression exp)
    {
        exp.Expression.Accept(this);
        exp.Type = exp.Expression.Type;
        return this;
    }
}
