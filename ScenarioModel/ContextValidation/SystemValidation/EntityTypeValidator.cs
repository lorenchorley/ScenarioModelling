using ScenarioModelling.ContextValidation.Errors;
using ScenarioModelling.ContextValidation.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects;

namespace ScenarioModelling.ContextValidation.SystemValidation;

[ObjectLike<IObjectValidator, EntityType>]
public class EntityTypeValidator : IObjectValidator<EntityType>
{
    public ValidationErrors Validate(System system, EntityType entity)
    {
        ValidationErrors validationErrors = new();
        // Initial state is of type StateMachine

        return validationErrors;
    }
}