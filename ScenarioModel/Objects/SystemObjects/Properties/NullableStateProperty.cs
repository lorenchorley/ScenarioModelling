using OneOf;
using ScenarioModel.Objects.SystemObjects.States;
using ScenarioModel.References;

namespace ScenarioModel.Objects.SystemObjects.Properties;

public class NullableStateProperty(System System)
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
                    () => throw new Exception("State reference could not be resolved.")
                )
            );
        }
    }
}
