namespace ScenarioModelling.ContextValidation.Errors;

public class InvalidPropertyValue(string message) : ValidationError(message)
{

}