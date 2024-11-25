using ScenarioModel.ContextValidation.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.Validation;

namespace ScenarioModel.ContextValidation.SystemValidation;

[ObjectLike<IObjectValidator, StateMachine>]
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