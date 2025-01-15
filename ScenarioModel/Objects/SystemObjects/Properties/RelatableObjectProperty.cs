using ScenarioModelling.Objects.SystemObjects.Interfaces;
using ScenarioModelling.References.Interfaces;

namespace ScenarioModelling.Objects.SystemObjects.Properties;

public class RelatableObjectProperty(System System) : OptionalReferencableProperty<IRelatable, IRelatableObjectReference>(System)
{
    public override string? Name
    {
        get => _valueOrReference?.Match(
            state => state.Name,
            reference => reference.Name
        );
    }
}
