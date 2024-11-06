using ScenarioModel.Objects.SystemObjects.Entities;

namespace ScenarioModel.Objects.SystemObjects.States;

public record State : INameful
{
    public string Name { get; set; } = "";
    public StateMachine StateMachine { get; set; } = null!;
    public List<Transition> Transitions { get; set; } = new();

    public bool TryTransition(string transitionName, IStateful stateful)
    {
        var transition = Transitions.FirstOrDefault(t => t.Name.IsEqv(transitionName));

        if (transition == null)
        {
            return false;
        }

        // TODO Constraints

        stateful.State.Set(StateMachine.States.First(s => s.Name.IsEqv(transition.DestinationState)));

        // TODO Effects

        return true;
    }

    public bool IsEqv(State other)
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
            if (!other.Transitions.Any(a => a.IsEqv(transition)))
            {
                throw new Exception($"No equivalent transition '{transition.Name}' not found in state '{other.Name}'.");
            }
        }

        return true;
    }
}
