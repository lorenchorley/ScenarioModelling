using ScenarioModelling.References;

namespace ScenarioModelling.Objects.SystemObjects.Properties;

public class StateProperty(System System) : OptionalReferencableProperty<State, StateReference>(System)
{
    public override string? Name
        => _valueOrReference?.Match(
            state => state.Name,
            reference => reference.Name
        );
}
