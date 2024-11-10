using ScenarioModel.Objects.SystemObjects;

namespace ScenarioModel.Validation;

public class AspectValidator : IValidator<Aspect>, ITypeValidator<AspectType>
{
    public ValidationErrors Validate(Aspect aspect)
    {
        ValidationErrors validationErrors = new();

        return validationErrors;
    }

    public ValidationErrors ValidateType(AspectType aspectType)
    {
        ValidationErrors validationErrors = new();

        return validationErrors;
    }
}