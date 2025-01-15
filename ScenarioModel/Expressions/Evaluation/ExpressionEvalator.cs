using ScenarioModelling.Expressions.Common;
using ScenarioModelling.Expressions.SemanticTree;
using ScenarioModelling.Expressions.Traversal;
using ScenarioModelling.Objects.SystemObjects;
using ScenarioModelling.References;
using ScenarioModelling.References.GeneralisedReferences;

namespace ScenarioModelling.Expressions.Evaluation;

public class ExpressionEvalator : IExpressionVisitor
{
    private readonly System _system;

    public ExpressionEvalator(System system)
    {
        _system = system;
    }

    public object VisitAnd(AndExpression exp)
    {
        var leftResult = (bool)exp.Left.Accept(this);
        var rightResult = (bool)exp.Right.Accept(this);

        return leftResult && rightResult;
    }

    public object VisitOr(OrExpression exp)
    {
        var leftResult = (bool)exp.Left.Accept(this);
        var rightResult = (bool)exp.Right.Accept(this);

        return leftResult || rightResult;
    }

    public object VisitHasRelation(HasRelationExpression exp)
    {
        CompositeValueObjectReference leftReference = new(_system)
        {
            Identifier = exp.Left
        };

        CompositeValueObjectReference rightReference = new(_system)
        {
            Identifier = exp.Right
        };

        var leftValueResolved = leftReference.ResolveReference();
        var rightValueResolved = rightReference.ResolveReference();

        if (leftValueResolved.IsNone)
        {
            throw new Exception($"Relatable object not found in system : {exp.Left}");
        }

        if (rightValueResolved.IsNone)
        {
            throw new Exception($"Relatable object not found in system : {exp.Right}");
        }

        RelationReference relationReference = new RelationReference(_system)
        {
            Name = exp.Name ?? "",
            Left = exp.Left,
            Right = exp.Right
        };

        var relations = relationReference.ResolveReference();
        return relations.IsSome;
    }

    public object VisitDoesNotHaveRelation(DoesNotHaveRelationExpression exp)
    {
        CompositeValueObjectReference leftReference = new(_system)
        {
            Identifier = exp.Left
        };

        CompositeValueObjectReference rightReference = new(_system)
        {
            Identifier = exp.Right
        };

        var leftValueResolved = leftReference.ResolveReference();
        var rightValueResolved = rightReference.ResolveReference();

        if (leftValueResolved.IsNone)
        {
            throw new Exception($"Relatable object not found in system : {exp.Left}");
        }

        if (rightValueResolved.IsNone)
        {
            throw new Exception($"Relatable object not found in system : {exp.Right}");
        }

        RelationReference relationReference = new RelationReference(_system)
        {
            Name = exp.Name ?? "",
            Left = exp.Left,
            Right = exp.Right
        };

        var relations = relationReference.ResolveReference();
        return relations.IsNone;
    }

    public object VisitCompositeValue(CompositeValue value)
    {
        var reference = new CompositeValueObjectReference(_system)
        {
            Identifier = value
        };

        var resolvedValue = reference.ResolveReference();
        if (resolvedValue.IsNone)
        {
            if (value.ValueList.Count > 1)
            {
                throw new Exception("Relatable object not found in system");
            }

            string stringValue = value.ValueList[0];

            if (stringValue.IsEqv("true"))
            {
                return true;
            }
            else if (stringValue.IsEqv("false"))
            {
                return false;
            }
            else
            {
                return stringValue;
            }
        }

        if (resolvedValue.Case is Entity entity)
        {
            return entity;
        }
        else if (resolvedValue.Case is Aspect aspect)
        {
            return aspect;
        }
        else if (resolvedValue.Case is Relation relation)
        {
            return relation;
        }
        else if (resolvedValue.Case is State state)
        {
            return state;
        }
        else
        {
            throw new Exception("Unsupported type for value composite");
        }
    }

    public object VisitEmpty(EmptyExpression exp)
    {
        throw new NotImplementedException();
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
        object leftResult = exp.Left.Accept(this);
        object rightResult = exp.Right.Accept(this);

        return !AreEqual(leftResult, rightResult);
    }

    public object VisitEqual(EqualExpression exp)
    {
        object leftResult = exp.Left.Accept(this);
        object rightResult = exp.Right.Accept(this);

        return AreEqual(leftResult, rightResult);
    }

    private bool AreEqual(object leftResult, object rightResult)
        => EqualityFunctions.EqualityTypeCases(
            leftResult,
            rightResult,
            _system,
            CompareStateAndString,
            (b1, b2) => b1 == b2,
            (s1, s2) => s1.IsEqv(s2),
            (e1, e2) => e1.Name.IsEqv(e2.Name)
        );

    private bool CompareStateAndString(State state, string str)
    {
        return str.IsEqv(state.Name);
    }

    public object VisitErroneousExpression(ErroneousExpression exp)
    {
        throw new NotImplementedException();
    }

    public object VisitBrackets(BracketsExpression exp)
    {
        return exp.Expression.Accept(this);
    }
}
