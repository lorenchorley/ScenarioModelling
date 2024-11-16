using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.References;

namespace ScenarioModel.Objects.SystemObjects;

public record State : ISystemObject
{
    private readonly System _system;

    public string Name { get; set; } = "";
    public Type Type => typeof(State);

    public StateMachine? _stateMachine;
    internal bool HasStateMachine => _stateMachine != null;
    public StateMachine StateMachine
    {
        get
        {
            if (_stateMachine == null)
            {
                _stateMachine =
                    _system.StateMachines
                           .FirstOrDefault(sm => sm.States.Any(s => s.Name.IsEqv(Name)))
                           ?? throw new Exception($"No corresponding state machine found for state '{Name}'.");
            }

            return _stateMachine;
        }
    }

    public IEnumerable<Transition> Transitions
    {
        get => StateMachine.Transitions
                           .Where(t => t.SourceState.IsEqv(this) || t.DestinationState.IsEqv(this)); // TODO Cache
    }

    public State(System system)
    {
        _system = system;

        // Add this to the system
        _system.States.Add(this);
    }

    public bool TryTransition(string transitionName, IStateful stateful)
    {
        var transition = Transitions.FirstOrDefault(t => t.Name.IsEqv(transitionName));

        if (transition == null)
        {
            return false;
        }

        // TODO Constraints

        string? name = transition.DestinationState.Name;

        if (name == null)
            throw new Exception("Transition destination state name is not set.");

        stateful.State.SetValue(StateMachine.States.First(s => name?.IsEqv(s.Name) ?? false));

        // TODO Effects

        return true;
    }

    public StateReference GenerateReference()
        => new StateReference(_system) { Name = Name };

    public bool IsDeepEqv(State other)
    {
        if (!Name.IsEqv(other.Name))
        {
            return false;
        }

        if (!StateMachine.Name.IsEqv(other.StateMachine.Name))
        {
            return false;
        }

        // There must be an equivalent aspect for each aspect, not complete but good enough perhaps
        foreach (var transition in Transitions)
        {
            if (!other.Transitions.Any(a => a.IsDeepEqv(transition)))
            {
                throw new Exception($"No equivalent transition '{transition.Name}' not found in state '{other.Name}'.");
            }
        }

        return true;
    }
}
