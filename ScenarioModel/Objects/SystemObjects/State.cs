using Newtonsoft.Json;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects.Interfaces;
using ScenarioModelling.Objects.Visitors;
using ScenarioModelling.References;

namespace ScenarioModelling.Objects.SystemObjects;

[SystemObjectLike<ISystemObject, State>]
public record State : ISystemObject<StateReference>
{
    private readonly System _system;

    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(State);

    private StateMachine? _stateMachine;
    internal bool HasStateMachine => _stateMachine != null;

    public string StateMachineName => StateMachine.Name;

    [JsonIgnore]
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

    public void DoTransition(string transitionName, IStateful statefulObject)
    {
        // Get the current state name so that we know which transitions have it as a source state
        var currentStateName = statefulObject.State.ResolvedValue?.Name
                               ?? throw new Exception("Stateful object state is not set.");

        var matchingTransitions = Transitions.Where(t => IsTransitionMatch(t, transitionName, currentStateName))
                                             .ToList();

        if (matchingTransitions.Count == 0)
        {
            throw new Exception($"No transition found for transition name '{transitionName}' and source state name '{currentStateName}'.");
        }
        else if (matchingTransitions.Count >= 2)
        {
            throw new Exception($"Multiple transitions found for transition name '{transitionName}' and source state name '{currentStateName}'.");
        }

        // We can assume that we have exactly one valid transition
        var transition = matchingTransitions[0];

        // TODO Constraints

        string destinationStateName = transition.DestinationState.Name
                                      ?? throw new Exception("Transition destination state name is not set.");

        // Find the actual state that corresponds to the state that is specified on the transition
        // We should verify that the state exists as early as possible to make debugging easier
        State newState = StateMachine.States.Single(s => destinationStateName.IsEqv(s.Name));
        statefulObject.State.SetValue(newState);

        // TODO Effects

    }

    private static bool IsTransitionMatch(Transition t, string transitionName, string sourceStateName)
    {
        // If transition names don't correspond, it's not our transition
        if (!t.Name.IsEqv(transitionName))
            return false;

        var transitionSourceState = t.SourceState.ResolvedValue;

        if (transitionSourceState == null)
            throw new Exception("Transition source state is not set.");

        // The source state of the transition must be the same as the source state of the transition request
        if (!transitionSourceState.Name.IsEqv(sourceStateName))
            return false;

        // If everything passes, then it must be a valid match
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

    public object Accept(ISystemVisitor visitor)
        => visitor.VisitState(this);
}
