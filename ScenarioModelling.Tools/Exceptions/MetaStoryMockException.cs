namespace ScenarioModelling.Tools.Exceptions;
public class MetaStoryMockException : Exception
{
    public MetaStoryMockException(string message) : base($"A mock error occured : {message}")
    {

    }
}
