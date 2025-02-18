namespace ScenarioModelling.Tools.Exceptions;
public class ExecutionException : Exception
{
    public ExecutionException(string message) : base($"A dialog execution error occured : {message}")
    {

    }
}
