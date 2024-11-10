using ScenarioModel.Expressions.Evaluation;
using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.Expressions.Traversal;
using ScenarioModel.Objects.SystemObjects;
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
        exp.Type = ExpressionValueType.Boolean;

        if (exp.Left.Type != ExpressionValueType.Boolean)
            Errors.Add(new ValidationError($"Left expression type in And Expression is not boolean : {exp.Left.Type}"));

        if (exp.Right.Type != ExpressionValueType.Boolean)
            Errors.Add(new ValidationError($"Right expression type in And Expression is not boolean : {exp.Right.Type}"));

        return this;
    }

    public object VisitOr(OrExpression exp)
    {
        exp.Type = ExpressionValueType.Boolean;

        if (exp.Left.Type != ExpressionValueType.Boolean)
            Errors.Add(new ValidationError($"Left expression type in Or Expression is not boolean : {exp.Left.Type}"));

        if (exp.Right.Type != ExpressionValueType.Boolean)
            Errors.Add(new ValidationError($"Right expression type in Or Expression is not boolean : {exp.Right.Type}"));

        return this;
    }

    public object VisitHasRelation(HasRelationExpression exp)
    {
        exp.Type = ExpressionValueType.Boolean;

        // Relation name must be valid in the given system even if not applied to something
        //var reference = new RelationReference()
        //{
        //    RelationName = exp.Name,
        //    FirstRelatableName = exp.Left,
        //    SecondRelatableName = exp.Right
        //};

        //if (reference.ResolveReference(_system).IsNone)
        //    Errors.Add(new ValidationError($"Relation {exp.Name} not found in system"));

        if (!IsRelatableType(exp.Left.Type))
            Errors.Add(new ValidationError($"Object type (Left) in Has Relation Expression is not a relatable object type : {exp.Left.Type}"));

        // Relatable object must exist in the system
        var firstRelatableReference = new CompositeValueObjectReference(_system)
        {
            Identifier = exp.Left
        };

        if (firstRelatableReference.ResolveReference().IsNone)
            Errors.Add(new ValidationError($"Relatable object {exp.Left} not found in system"));


        if (exp.Right.Type != ExpressionValueType.Relation)
            Errors.Add(new ValidationError($"Relation name (Right) in Has Relation Expression does not resolve to a relation : {exp.Right.Type}"));

        var secondRelatableReference = new CompositeValueObjectReference(_system)
        {
            Identifier = exp.Right
        };

        // Reference object must exist in the system
        if (secondRelatableReference.ResolveReference().IsNone)
            Errors.Add(new ValidationError($"Relatable object {exp.Right} not found in system"));

        return this;
    }

    private static bool IsRelatableType(ExpressionValueType type)
        => type == ExpressionValueType.Entity || type == ExpressionValueType.Aspect;

    public object VisitDoesNotHaveRelation(DoesNotHaveRelationExpression exp)
    {
        exp.Type = ExpressionValueType.Boolean;

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


        if (exp.Right.Type != ExpressionValueType.Relation)
            Errors.Add(new ValidationError($"Relation name (Right) in Does Not Have Relation Expression does not resolve to a relation : {exp.Right.Type}"));

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
                value.Type = ExpressionValueType.Unknown;
                Errors.Add(new ValidationError($"Relatable object '{value}' not found in system"));
                return this;
            }

            string stringValue = value.ValueList[0];

            if (stringValue.IsEqv("true") || stringValue.IsEqv("false"))
            {
                value.Type = ExpressionValueType.Boolean;
                return this;
            }
            else
            {
                // String
                value.Type = ExpressionValueType.String;

                return this;
            }
        }

        if (referencedValue.Case is Entity)
        {
            value.Type = ExpressionValueType.Entity;
        }
        else if (referencedValue.Case is Aspect)
        {
            value.Type = ExpressionValueType.Aspect;
        }
        else if (referencedValue.Case is Relation)
        {
            value.Type = ExpressionValueType.Relation;
        }
        else if (referencedValue.Case is State)
        {
            value.Type = ExpressionValueType.State;
        }
        else
        {
            value.Type = ExpressionValueType.Unknown;
            Errors.Add(new ValidationError($"Relatable object '{value}' found but of unknown type '{referencedValue.Case.GetType().Name}'"));
        }

        return this;
    }

    public object VisitEmpty(EmptyExpression exp)
    {
        exp.Type = ExpressionValueType.Unknown;

        Errors.Add(new ValidationError("Expression was empty"));

        return this;
    }

    public object VisitArgumentList(ArgumentList list)
    {
        return this;
    }

    public object VisitFunction(FunctionExpression exp)
    {
        exp.Type = ExpressionValueType.Unknown;
        return this;
    }

    public object VisitNotEqual(NotEqualExpression exp)
    {
        exp.Type = ExpressionValueType.Boolean;

        if (exp.Left.Type != exp.Right.Type)
        {
            Errors.Add(new ValidationError($"Left and right expression types in Non-Equality Expression are diffents : {exp.Left.Type} != {exp.Right.Type}"));
        }

        return this;
    }

    public object VisitEqual(EqualExpression exp)
    {
        exp.Type = ExpressionValueType.Boolean;

        if (exp.Left.Type != exp.Right.Type)
        {
            Errors.Add(new ValidationError($"Left and right expression types in Equality Expression are diffents : {exp.Left.Type} != {exp.Right.Type}"));
        }

        return this;
    }

    public object VisitErroneousExpression(ErroneousExpression exp)
    {
        return this;
    }

    public object VisitBrackets(BracketsExpression exp)
    {
        exp.Type = exp.Expression.Type;
        return this;
    }
}
