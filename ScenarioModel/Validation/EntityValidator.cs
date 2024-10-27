using ScenarioModel.SystemObjects.Entities;

namespace ScenarioModel.Validation;

public class EntityValidator : IValidator<Entity>, ITypeValidator<EntityType>
{
    public ValidationErrors Validate(Entity entity)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }

    public ValidationErrors ValidateType(EntityType instance)
    {
        ValidationErrors validationErrors = new();

        // Initial state is of type StateMachine

        return validationErrors;
    }
}