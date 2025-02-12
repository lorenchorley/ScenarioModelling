using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.SystemObjects.Interfaces;

namespace ScenarioModelling.CoreObjects.SystemObjects.Properties;

public class RelatableObjectProperty(MetaState System) : OptionalReferencableProperty<IRelatable, IRelatableObjectReference>(System)
{
    public override string? Name
    {
        get => _valueOrReference?.Match(
            state => state.Name,
            reference => reference.Name
        );
    }
}
