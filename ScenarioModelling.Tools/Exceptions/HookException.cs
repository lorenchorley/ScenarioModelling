namespace ScenarioModelling.Tools.Exceptions;
public class HookException : Exception
{
    public HookException(string message) : base($"A code hook was improperly used : {message}")
    {

    }
}
