using ScenarioModelling.CoreObjects.References;

namespace ScenarioModelling.CoreObjects.MetaStateObjects.Properties;

public class StateProperty(MetaState System) : OptionalReferencableProperty<State, StateReference>(System)
{
    public override string? Name
        => _valueOrReference?.Match(
            state => state.Name,
            reference => reference.Name
        );
}
