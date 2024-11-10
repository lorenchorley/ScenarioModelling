﻿using LanguageExt;
using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.Objects.SystemObjects.Properties;
using ScenarioModel.References;

namespace ScenarioModel.Objects.SystemObjects;

/// <summary>
/// Defines the state machine for a state, allows for reuse and analysis 
/// </summary>
public record StateMachine : IIdentifiable
{
    private readonly System _system;

    public string Name { get; set; } = "";
    public Type Type => typeof(StateMachine);

    public StateListProperty States { get; set; }
    public TransitionListProperty Transitions { get; set; }

    public StateMachine(System system)
    {
        _system = system;

        // Add this to the system
        system.StateMachines.Add(this);

        States = new(system);
        Transitions = new(system);
    }

    public StateMachineReference GenerateReference()
        => new StateMachineReference(_system) { Name = Name };

    internal void AssertDeepEqv(StateMachine other)
    {
        if (!Name.IsEqv(other.Name))
            throw new Exception($"State machine names do not match: '{Name}' and '{other.Name}'.");

        if (States.Count != other.States.Count)
            throw new Exception($"State machine '{Name}' has {States.Count} relations, but state machine '{other.Name}' has {other.States.Count}.");

        // There must be an equivalent state for each state, not complete but good enough perhaps
        foreach (var state in States)
        {
            if (!other.States.Any(r => r.IsDeepEqv(state)))
            {
                throw new Exception($"No equivalent state '{state.Name}' not found in state machine '{other.Name}'.");
            }
        }

        // There must be an equivalent aspect for each aspect, not complete but good enough perhaps
        foreach (var transition in Transitions)
        {
            if (!other.Transitions.Any(a => a.IsDeepEqv(transition)))
            {
                throw new Exception($"No equivalent transition '{transition.Name}' not found in state machine '{other.Name}'.");
            }
        }
    }

}