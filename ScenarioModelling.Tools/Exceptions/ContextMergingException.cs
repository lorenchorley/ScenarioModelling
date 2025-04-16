namespace ScenarioModelling.Tools.Exceptions;
public class ContextMergingException : Exception
{
    public ContextMergingException(string message) : base($"A merging error occured : {message}")
    {

    }
}
