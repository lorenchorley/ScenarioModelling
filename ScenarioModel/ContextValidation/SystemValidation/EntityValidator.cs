using ScenarioModelling.ContextValidation.Errors;
using ScenarioModelling.ContextValidation.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects;

namespace ScenarioModelling.ContextValidation.SystemValidation;

[SystemObjectLike<IObjectValidator, Entity>]
public class EntityValidator : IObjectValidator<Entity>
{
    public ValidationErrors Validate(System system, Entity entity)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}