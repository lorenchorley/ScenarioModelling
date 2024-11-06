
using ScenarioModel.Objects.SystemObjects.Entities;

namespace ScenarioModel.Objects.SystemObjects.States;

/// <summary>
/// Defines the state machine for a state, allows for reuse and analysis 
/// </summary>
public record StateMachine : INameful
{
    public string Name { get; set; } = "";
    public List<State> States { get; set; } = new();
    public List<Transition> Transitions { get; set; } = new();

    public StateMachine(System system)
    {
        // Add this entity to the system
        system.StateMachines.Add(this);
    }

    internal void AssertEqv(StateMachine other)
    {
        if (!Name.IsEqv(other.Name))
            throw new Exception($"State machine names do not match: '{Name}' and '{other.Name}'.");

        if (States.Count != other.States.Count)
            throw new Exception($"State machine '{Name}' has {States.Count} relations, but state machine '{other.Name}' has {other.States.Count}.");

        // There must be an equivalent state for each state, not complete but good enough perhaps
        foreach (var state in States)
        {
            if (!other.States.Any(r => r.IsEqv(state)))
            {
                throw new Exception($"No equivalent state '{state.Name}' not found in state machine '{other.Name}'.");
            }
        }

        // There must be an equivalent aspect for each aspect, not complete but good enough perhaps
        foreach (var transition in Transitions)
        {
            if (!other.Transitions.Any(a => a.IsEqv(transition)))
            {
                throw new Exception($"No equivalent transition '{transition.Name}' not found in state machine '{other.Name}'.");
            }
        }
    }
}
