namespace ScenarioModel.ContextValidation.Errors;

public class InvalidPropertyValue(string message) : ValidationError(message)
{

}