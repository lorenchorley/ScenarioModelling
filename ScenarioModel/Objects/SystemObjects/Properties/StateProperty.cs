using ScenarioModel.References;

namespace ScenarioModel.Objects.SystemObjects.Properties;

public class StateProperty(System System) : OptionalReferencableProperty<State, StateReference>(System)
{
    public override string? Name
    {
        get
        {
            return _valueOrReference?.Match(
                state => state.Name,
                reference => reference.Name
            );
        }
    }
}
