using LanguageExt;
using OneOf;
using ScenarioModel.Objects.SystemObjects.States;
using ScenarioModel.References;

namespace ScenarioModel.Objects.SystemObjects.Properties;

public class StateProperty(System System)
{
    private OneOf<State, StateReference>? _state = null;

    public void Set(State? state)
    {
        if (state == null)
        {
            _state = null;
        }
        else
        {
            _state = state;
        }
    }

    public void Set(StateReference reference)
    {
        _state = reference;
    }

    public string? StateName
    {
        get
        {
            return _state?.Match(
                state => state.Name,
                reference => reference.StateName
            );
        }
    }

    public State? OnlyCompleteValue
    {
        get => _state?.Match<State?>(
                state => state,
                reference => null
            );
    }
    
    public StateReference? OnlyReference
    {
        get => _state?.Match<StateReference?>(
                state => null,
                reference => reference
            );
    }

    public State? ResolvedValue
    {
        get
        {
            if (_state == null)
                return null;

            return ((OneOf<State, StateReference>)_state).Match(
                state => state,
                reference => reference.ResolveReference(System).Match(
                    state => state,
                    () => throw new Exception($"State reference '{reference.StateName}' could not be resolved.")
                )
            );
        }
    }

    public bool IsEqv(StateProperty other)
    {
        if ((_state == null && other._state != null) ||
            (_state != null && other._state == null))
        {
            return false;
        }

        if (_state == null && other._state == null)
        {
            return true;
        }

        return _state.Value.Match(
            state => other._state.Value.Match(
                otherState => state.Name.IsEqv(otherState.Name),
                otherReference => false),
            reference => other._state.Value.Match(
                otherState => false,
                otherReference => reference.StateName.IsEqv(otherReference.StateName)));
    }
}
