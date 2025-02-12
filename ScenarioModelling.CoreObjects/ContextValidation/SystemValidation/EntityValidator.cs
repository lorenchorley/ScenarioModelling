using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.SystemObjects;

namespace ScenarioModelling.CoreObjects.ContextValidation.SystemValidation;

[SystemObjectLike<IObjectValidator, Entity>]
public class EntityValidator : IObjectValidator<Entity>
{
    public ValidationErrors Validate(MetaState system, Entity entity)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}