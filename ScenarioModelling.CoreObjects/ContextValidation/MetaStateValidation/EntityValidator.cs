using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects;

namespace ScenarioModelling.CoreObjects.ContextValidation.MetaStateValidation;

[MetaStateObjectLike<IObjectValidator, Entity>]
public class EntityValidator : IObjectValidator<Entity>
{
    public ValidationErrors Validate(MetaState system, Entity entity)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}