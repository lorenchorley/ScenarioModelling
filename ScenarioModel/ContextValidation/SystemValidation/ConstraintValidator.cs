using ScenarioModelling.ContextValidation.Errors;
using ScenarioModelling.ContextValidation.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Expressions.Initialisation;
using ScenarioModelling.Objects.SystemObjects;

namespace ScenarioModelling.ContextValidation.SystemValidation;

[SystemObjectLike<IObjectValidator, Constraint>]
public class ConstraintValidator : IObjectValidator<Constraint>
{
    public ValidationErrors Validate(System system, Constraint constraint)
    {
        ExpressionInitialiser _visitor = new ExpressionInitialiser(system);
        constraint.Condition.Accept(_visitor);
        return _visitor.Errors;
    }
}