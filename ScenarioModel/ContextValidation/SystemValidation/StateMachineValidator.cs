using ScenarioModelling.ContextValidation.Errors;
using ScenarioModelling.ContextValidation.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects;

namespace ScenarioModelling.ContextValidation.SystemValidation;

[SystemObjectLike<IObjectValidator, StateMachine>]
public class StateMachineValidator : IObjectValidator<StateMachine>
{
    public ValidationErrors Validate(System system, StateMachine state)
    {
        ValidationErrors validationErrors = new();

        // All states should be of this type
        // All states should be valid

        return validationErrors;
    }

}