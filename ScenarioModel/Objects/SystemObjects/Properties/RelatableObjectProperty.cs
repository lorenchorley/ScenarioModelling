using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.References;

namespace ScenarioModel.Objects.SystemObjects.Properties;

public class RelatableObjectProperty(System System) : OptionalReferencableProperty<IRelatable, RelatableObjectReference>(System)
{
    public override string? Name
    {
        get => _valueOrReference?.Match(
            state => state.Name,
            reference => reference.Name
        );
    }

}
