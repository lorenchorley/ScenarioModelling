﻿using LanguageExt;
using Newtonsoft.Json;
using ProtoBuf;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects.Properties;
using ScenarioModelling.CoreObjects.Visitors;
using YamlDotNet.Serialization;

namespace ScenarioModelling.CoreObjects.MetaStateObjects;

/// <summary>
/// Defines the state machine for a state, allows for reuse and analysis 
/// </summary>
[ProtoContract]
[MetaStateObjectLike<ISystemObject, StateMachine>]
public record StateMachine : ISystemObject<StateMachineReference>, IOptionalSerialisability
{
    private MetaState _system = null!;

    [JsonIgnore]
    [YamlIgnore]
    public Type Type => typeof(StateMachine);

    [ProtoMember(1)]
    public string Name { get; set; } = "";

    [ProtoMember(2)]
    public StateListProperty States { get; set; }

    [ProtoMember(3)]
    public TransitionListProperty Transitions { get; set; }

    public bool ExistanceOriginallyInferred { get; set; } = false;
    public bool ShouldReserialise
    {
        get
        {
            if (ExistanceOriginallyInferred)
                return false;

            //if (States.HasValues)
            //    return false;

            //if (Transitions.HasValues)
            //    return false;

            return true;
        }
    }

    private StateMachine()
    {

    }

    public StateMachine(MetaState system)
    {
        _system = system;

        // Add this to the system
        system.StateMachines.Add(this);

        States = new(system);
        Transitions = new(system);
    }

    public void InitialiseAfterDeserialisation(MetaState system)
    {
        _system = system;
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

    public object Accept(IMetaStateVisitor visitor)
        => visitor.VisitStateMachine(this);
}
