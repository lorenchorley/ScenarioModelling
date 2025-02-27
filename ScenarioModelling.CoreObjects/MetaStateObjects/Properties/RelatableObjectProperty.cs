using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;

namespace ScenarioModelling.CoreObjects.MetaStateObjects.Properties;

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
