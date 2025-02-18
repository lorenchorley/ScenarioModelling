namespace ScenarioModelling.Tools.Exceptions;

public class ExpressionException : Exception
{
    public ExpressionException(string message) : base($"An error occured within an expression : {message}")
    {

    }
}
