namespace ScenarioModel.Objects.SystemObjects.States;

/// <summary>
/// Defines the state machine for a state, allows for reuse and analysis 
/// </summary>
public record StateMachine : INameful
{
    public string Name { get; set; } = "";
    public List<State> States { get; set; } = new();
    public List<Transition> Transitions { get; set; } = new();
}
