using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.Validation;

public class StateValidator : IValidator<State>, ITypeValidator<StateType>
{
    public ValidationErrors Validate(State state)
    {
        ValidationErrors validationErrors = new();

        // All transitions should point to states of the same type
        foreach (var transition in state.Transitions)
        {
            validationErrors.AddIfNot(state.StateType.States.Any(s => string.Equals(s.Name, transition)), new UnknownStateTransition($"State {state.Name} has transition to state {transition} that is not part of the state's type {state.StateType.Name}"));
        }

        return validationErrors;
    }

    public ValidationErrors ValidateType(StateType stateType)
    {
        ValidationErrors validationErrors = new();

        // All states should be of this type
        // All states should be valid

        return validationErrors;
    }
}