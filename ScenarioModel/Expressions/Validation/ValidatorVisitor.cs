using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.Expressions.Traversal;
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

    public object VisitAndConstraint(AndExpression andConstraint)
    {
        return Errors;
    }

    public object VisitOrConstraint(OrExpression orConstraint)
    {
        return Errors;
    }

    public object VisitHasRelationConstraint(HasRelationExpression hasRelationConstraint)
    {
        hasRelationConstraint.Ref.ResolveReference(_system).Match(
            Some: _ => { },
            None: () => Errors.Add(new ValidationError($"Relation {hasRelationConstraint.Ref.RelationName} not found."))
            );

        return Errors;
    }
}
