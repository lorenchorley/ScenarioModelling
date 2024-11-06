namespace ScenarioModel.Objects.SystemObjects.States;

public record Transition
{
    public string Name { get; set; } = "";
    public string SourceState { get; set; } = "";
    public string DestinationState { get; set; } = "";

    public bool IsEqv(Transition other)
    {
        return Name.IsEqv(other.Name) &&
               SourceState.IsEqv(other.SourceState) &&
               DestinationState.IsEqv(other.DestinationState);
    }
}