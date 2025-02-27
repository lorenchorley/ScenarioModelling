using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects;

namespace ScenarioModelling.CoreObjects.ContextValidation.MetaStateValidation;

[MetaStateObjectLike<IObjectValidator, Transition>]
public class TransitionValidator : IObjectValidator<Transition>
{
    public ValidationErrors Validate(MetaState system, Transition transition)
    {
        ValidationErrors validationErrors = new();


        return validationErrors;
    }
}