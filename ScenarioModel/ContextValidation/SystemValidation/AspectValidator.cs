using ScenarioModel.ContextValidation.Errors;
using ScenarioModel.ContextValidation.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.SystemObjects;

namespace ScenarioModel.ContextValidation.SystemValidation;

[ObjectLike<IObjectValidator, Aspect>]
public class AspectValidator : IObjectValidator<Aspect>
{
    public ValidationErrors Validate(System system, Aspect aspect)
    {
        ValidationErrors validationErrors = new();

        return validationErrors;
    }

}