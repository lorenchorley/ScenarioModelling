namespace ScenarioModel.Validation;

public class ScenarioValidator : IValidator<Scenario>
{
    private SystemValidator _systemValidator = new();
    private StepGraphValidator _stepGraphValidator = new();

    public ValidationErrors Validate(Scenario scenario)
    {
        ValidationErrors validationErrors = new();

        // Name is unique among scenarios
        // InitialStep is in the step graph

        validationErrors.Incorporate(_stepGraphValidator.Validate(scenario.Steps));
        validationErrors.Incorporate(_systemValidator.Validate(scenario.System));

        return validationErrors;
    }
}