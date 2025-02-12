namespace ScenarioModelling.CoreObjects.ContextValidation.Errors;

public class InvalidPropertyValue(string message) : ValidationError(message)
{

}