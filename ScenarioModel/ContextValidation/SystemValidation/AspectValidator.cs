using ScenarioModelling.ContextValidation.Errors;
using ScenarioModelling.ContextValidation.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects;

namespace ScenarioModelling.ContextValidation.SystemValidation;

[ObjectLike<IObjectValidator, Aspect>]
public class AspectValidator : IObjectValidator<Aspect>
{
    public ValidationErrors Validate(System system, Aspect aspect)
    {
        ValidationErrors validationErrors = new();

        return validationErrors;
    }

}