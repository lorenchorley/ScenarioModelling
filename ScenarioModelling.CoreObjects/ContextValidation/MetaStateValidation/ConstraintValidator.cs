using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.Expressions.Initialisation;
using ScenarioModelling.CoreObjects.MetaStateObjects;

namespace ScenarioModelling.CoreObjects.ContextValidation.MetaStateValidation;

[MetaStateObjectLike<IObjectValidator, Constraint>]
public class ConstraintValidator : IObjectValidator<Constraint>
{
    public ValidationErrors Validate(MetaState system, Constraint constraint)
    {
        ExpressionInitialiser _visitor = new ExpressionInitialiser(system);
        constraint.Condition.Accept(_visitor);
        return _visitor.Errors;
    }
}