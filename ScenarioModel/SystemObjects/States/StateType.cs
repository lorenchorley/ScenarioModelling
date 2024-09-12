namespace ScenarioModel.SystemObjects.States;

/// <summary>
/// Defines the state machine for a state, allows for reuse and analysis 
/// </summary>
public class StateType
{
    public string Name { get; set; } = "";
    public List<State> States { get; set; } = new();
}
