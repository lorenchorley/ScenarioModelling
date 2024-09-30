using ScenarioModel.SystemObjects.Constraints.Traversal;
using ScenarioModel.Validation;

namespace ScenarioModel.SystemObjects.Entities;

public class ConstraintValidatorVisitor : IConstraintNodeVisitor
{
    public ValidationErrors Errors { get; } = new();

    private readonly System _system;

    public ConstraintValidatorVisitor(System system)
    {
        _system = system;
    }

    public object VisitAndConstraint(AndConstraint andConstraint)
    {
        return Errors;
    }

    public object VisitOrConstraint(OrConstraint orConstraint)
    {
        return Errors;
    }

    public object VisitHasRelationConstraint(HasRelationConstraint hasRelationConstraint)
    {
        hasRelationConstraint.Ref.ResolveReference(_system).Match(
            Some: _ => { },
            None: () => Errors.Add(new ValidationError($"Relation {hasRelationConstraint.Ref.RelationName} not found."))
            );

        return Errors;
    }
}
