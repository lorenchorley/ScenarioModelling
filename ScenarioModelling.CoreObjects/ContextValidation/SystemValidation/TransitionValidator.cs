using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.SystemObjects;

namespace ScenarioModelling.CoreObjects.ContextValidation.SystemValidation;

[SystemObjectLike<IObjectValidator, Transition>]
public class TransitionValidator : IObjectValidator<Transition>
{
    public ValidationErrors Validate(MetaState system, Transition transition)
    {
        ValidationErrors validationErrors = new();


        return validationErrors;
    }
}