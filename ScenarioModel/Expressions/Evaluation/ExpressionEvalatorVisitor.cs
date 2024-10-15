﻿using LanguageExt;
using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.Expressions.Traversal;
using Relation = ScenarioModel.SystemObjects.Relations.Relation;

namespace ScenarioModel.Expressions.Evaluation;

public class ExpressionEvalatorVisitor : IExpressionVisitor
{
    private readonly System _system;
    private bool _currentEvaluationResult;

    public ExpressionEvalatorVisitor(System system)
    {
        _system = system;
    }

    public object VisitAndConstraint(AndExpression andConstraint)
    {
        var leftResult = (bool)andConstraint.Left.Accept(this);
        var rightResult = (bool)andConstraint.Right.Accept(this);

        return leftResult && rightResult;
    }

    public object VisitOrConstraint(OrExpression orConstraint)
    {
        var leftResult = (bool)orConstraint.Left.Accept(this);
        var rightResult = (bool)orConstraint.Right.Accept(this);

        return leftResult || rightResult;
    }

    public object VisitHasRelationConstraint(HasRelationExpression hasRelationConstraint)
    {
        Option<Relation> systemRelation = hasRelationConstraint.Ref.ResolveReference(_system);

        if (systemRelation.IsNone)
        {
            return false;
        }

        var relatables = hasRelationConstraint.RelatableObject.ResolveReference(_system);

        return relatables.Match(
            relatable =>
            {
                foreach (var objectRelation in relatable.Relations)
                {
                    if (systemRelation.Case == objectRelation)
                    {
                        return true;
                    }
                }

                return false;
            },
            () => false
        );
    }

}