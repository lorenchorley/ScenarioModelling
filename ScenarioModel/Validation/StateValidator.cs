using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.Validation;

public class StateValidator : IValidator<State>, ITypeValidator<StateMachine>
{
    public ValidationErrors Validate(State state)
    {
        ValidationErrors validationErrors = new();

        // All transitions should point to states of the same type
        foreach (var transition in state.Transitions)
        {
            validationErrors.AddIfNot(state.StateMachine.States.Any(s => s.Name.IsEqv(transition.Name)), new UnknownStateTransition($"State {state.Name} has transition to state {transition} that is not part of the state's type {state.StateMachine.Name}"));
        }

        return validationErrors;
    }

    public ValidationErrors ValidateType(StateMachine stateType)
    {
        ValidationErrors validationErrors = new();

        // All states should be of this type
        // All states should be valid

        return validationErrors;
    }
}