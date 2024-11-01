namespace ScenarioModel.Validation;

public class ScenarioValidator : IValidator<Scenario>
{
    private SystemValidator _systemValidator = new();
    private GraphValidator _nodeGraphValidator = new();

    public ValidationErrors Validate(Scenario scenario)
    {
        ValidationErrors validationErrors = new();

        // Name is unique among scenarios
        // InitialNode is in the graph

        validationErrors.Incorporate(_nodeGraphValidator.Validate(scenario.Graph.PrimarySubGraph));
        //validationErrors.Incorporate(_systemValidator.Validate(scenario.System));

        return validationErrors;
    }
}