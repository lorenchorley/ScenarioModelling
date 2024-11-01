﻿using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.Expressions.Traversal;
using ScenarioModel.Objects.SystemObjects.Entities;

namespace ScenarioModel.Expressions.Evaluation;

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

    public object VisitHasRelation(HasRelationExpression hasRelationConstraint)
    {
        //Option<Relation> systemRelation = hasRelationConstraint.Ref.ResolveReference(_system);

        //if (systemRelation.IsNone)
        //{
        //    return false;
        //}

        //var relatables = hasRelationConstraint.RelatableObject.ResolveReference(_system);

        //return relatables.Match(
        //    relatable =>
        //    {
        //        foreach (var objectRelation in relatable.Relations)
        //        {
        //            if (systemRelation.Case == objectRelation)
        //            {
        //                return true;
        //            }
        //        }

        //        return false;
        //    },
        //    () => false
        //);
        throw new NotImplementedException();
    }

    public object VisitValueComposite(ValueComposite value)
    {
        return value;
    }

    public object VisitEmpty(EmptyExpression exp)
    {
        throw new NotImplementedException();
    }

    public object VisitDoesNotHaveRelation(DoesNotHaveRelationExpression exp)
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
    {
        if (leftResult == null)
            throw new Exception("Left side of the equal expression is null");

        if (rightResult == null)
            throw new Exception("Right side of the equal expression is null");

        if (leftResult.GetType() != rightResult.GetType())
            throw new Exception("Cannot compare values of different types");

        if (leftResult.GetType() == typeof(bool))
        {
            return leftResult == rightResult;
        }

        if (leftResult.GetType() == typeof(string))
        {
            return ((string)leftResult).IsEqv((string)rightResult);
        }

        if (leftResult is ValueComposite leftValue && 
            rightResult is ValueComposite rightValue)
        {
            // Resolve values
            var leftValueResolved = _system.ResolveValue(leftValue);
            var rightValueResolved = _system.ResolveValue(rightValue);

            return AreEqual(leftValueResolved, rightValueResolved);
        }
        
        if (leftResult is Entity leftEntity && 
            rightResult is Entity rightEntity)
        {
            return leftEntity.Name.IsEqv(rightEntity.Name);
        }
        

        throw new Exception($"Unsupported type for equal expression : {leftResult.GetType().Name}");
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
