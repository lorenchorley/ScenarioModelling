using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.Expressions.Initialisation;
using ScenarioModelling.CoreObjects.SystemObjects;

namespace ScenarioModelling.CoreObjects.ContextValidation.SystemValidation;

[SystemObjectLike<IObjectValidator, Constraint>]
public class ConstraintValidator : IObjectValidator<Constraint>
{
    public ValidationErrors Validate(MetaState system, Constraint constraint)
    {
        ExpressionInitialiser _visitor = new ExpressionInitialiser(system);
        constraint.Condition.Accept(_visitor);
        return _visitor.Errors;
    }
}