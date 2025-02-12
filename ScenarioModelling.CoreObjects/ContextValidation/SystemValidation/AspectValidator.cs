using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.SystemObjects;

namespace ScenarioModelling.CoreObjects.ContextValidation.SystemValidation;

[SystemObjectLike<IObjectValidator, Aspect>]
public class AspectValidator : IObjectValidator<Aspect>
{
    public ValidationErrors Validate(MetaState system, Aspect aspect)
    {
        ValidationErrors validationErrors = new();

        return validationErrors;
    }

}