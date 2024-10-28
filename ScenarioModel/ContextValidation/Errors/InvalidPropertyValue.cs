namespace ScenarioModel.Validation;

public class InvalidPropertyValue(string message) : ValidationError(message)
{

}