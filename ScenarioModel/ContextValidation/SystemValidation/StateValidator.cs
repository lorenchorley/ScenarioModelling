using ScenarioModelling.ContextValidation.Errors;
using ScenarioModelling.ContextValidation.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects;

namespace ScenarioModelling.ContextValidation.SystemValidation;

[SystemObjectLike<IObjectValidator, State>]
public class StateValidator : IObjectValidator<State>
{
    public ValidationErrors Validate(System system, State state)
    {
        ValidationErrors validationErrors = new();

        // All transitions should point to states of the same type
        foreach (var transition in state.Transitions)
        {
            validationErrors.AddIfNot(state.StateMachine.States.Any(s => s.Name.IsEqv(transition.Name)), new UnknownStateTransition($"State {state.Name} has transition to state {transition} that is not part of the state's type {state.StateMachine.Name}"));
        }

        return validationErrors;
    }

}