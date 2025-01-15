using ScenarioModelling.ContextValidation.Errors;

namespace ScenarioModelling.ContextValidation;

public class ContextValidator
{
    public ValidationErrors Validate(Context context)
    {
        ValidationErrors validationErrors = new();
        ScenarioValidator scenarioValidator = new();
        SystemValidator systemValidator = new();

        foreach (var scenario in context.Scenarios)
        {
            validationErrors.Incorporate(scenarioValidator.Validate(context.System, scenario));
        }

        validationErrors.Incorporate(systemValidator.Validate(context.System));

        return validationErrors;
    }
}
