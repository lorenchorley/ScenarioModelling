using ScenarioModel.Objects.System;

namespace ScenarioModel.Objects.System.States;

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

        stateful.State = StateMachine.States.First(s => s.Name.IsEqv(transition.DestinationState));

        // TODO Effects

        return true;
    }
}
