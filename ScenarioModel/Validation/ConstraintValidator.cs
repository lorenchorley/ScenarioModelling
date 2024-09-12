using ScenarioModel.SystemObjects.Entities;

namespace ScenarioModel.Validation;

public class ConstraintValidator :  IValidator<Constraint>
{
    public ValidationErrors Validate(Constraint constraint)
    {
        ValidationErrors validationErrors = new();

        // If has state, it should be of type EntityType.StateType
        // All relations should be valid
        // This entity should be either the left or right entity of each relation

        return validationErrors;
    }
}