namespace ScenarioModelling.Tools.Exceptions;
public class InternalLogicException : Exception
{
    public InternalLogicException(string message) : base($"Arrived at an inconsistent internal state : {message}")
    {

    }
}
