using ScenarioModel.ContextValidation.Errors;
using ScenarioModel.ContextValidation.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.SystemObjects;

namespace ScenarioModel.ContextValidation.SystemValidation;

[ObjectLike<IObjectValidator, Entity>]
public class EntityValidator : IObjectValidator<Entity>
{
    public ValidationErrors Validate(System system, Entity entity)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}