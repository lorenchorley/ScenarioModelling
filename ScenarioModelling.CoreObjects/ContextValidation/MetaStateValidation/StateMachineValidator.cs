using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects;

namespace ScenarioModelling.CoreObjects.ContextValidation.MetaStateValidation;

[MetaStateObjectLike<IObjectValidator, StateMachine>]
public class StateMachineValidator : IObjectValidator<StateMachine>
{
    public ValidationErrors Validate(MetaState system, StateMachine state)
    {
        ValidationErrors validationErrors = new();

        // All states should be of this type
        // All states should be valid

        return validationErrors;
    }

}