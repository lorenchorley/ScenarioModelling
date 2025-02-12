using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.SystemObjects;

namespace ScenarioModelling.CoreObjects.ContextValidation.SystemValidation;

[SystemObjectLike<IObjectValidator, EntityType>]
public class EntityTypeValidator : IObjectValidator<EntityType>
{
    public ValidationErrors Validate(MetaState system, EntityType entity)
    {
        ValidationErrors validationErrors = new();
        // Initial state is of type StateMachine

        return validationErrors;
    }
}