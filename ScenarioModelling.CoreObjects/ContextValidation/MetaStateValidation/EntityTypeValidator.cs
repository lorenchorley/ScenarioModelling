using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects;

namespace ScenarioModelling.CoreObjects.ContextValidation.MetaStateValidation;

[MetaStateObjectLike<IObjectValidator, EntityType>]
public class EntityTypeValidator : IObjectValidator<EntityType>
{
    public ValidationErrors Validate(MetaState system, EntityType entity)
    {
        ValidationErrors validationErrors = new();
        // Initial state is of type StateMachine

        return validationErrors;
    }
}