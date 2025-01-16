using ScenarioModelling.ContextValidation.Errors;
using ScenarioModelling.ContextValidation.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects;

namespace ScenarioModelling.ContextValidation.SystemValidation;

[SystemObjectLike<IObjectValidator, Transition>]
public class TransitionValidator : IObjectValidator<Transition>
{
    public ValidationErrors Validate(System system, Transition transition)
    {
        ValidationErrors validationErrors = new();


        return validationErrors;
    }
}