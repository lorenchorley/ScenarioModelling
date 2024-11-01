namespace ScenarioModel.Objects.SystemObjects.States;

public record Transition
{
    public string Name { get; set; } = "";
    public string SourceState { get; set; } = "";
    public string DestinationState { get; set; } = "";
}