using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.References.Interfaces;

namespace ScenarioModel.Objects.SystemObjects.Properties;

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
