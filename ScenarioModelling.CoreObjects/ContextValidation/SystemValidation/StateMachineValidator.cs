using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.SystemObjects;

namespace ScenarioModelling.CoreObjects.ContextValidation.SystemValidation;

[SystemObjectLike<IObjectValidator, StateMachine>]
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