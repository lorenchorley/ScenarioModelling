using ScenarioModel.References;

namespace ScenarioModel.Objects.SystemObjects.Properties;

public class EntityTypeProperty(System System) : OptionalReferencableProperty<EntityType, EntityTypeReference>(System)
{
    public override string? Name
    {
        get
        {
            return _valueOrReference?.Match(
                entityType => entityType.Name,
                reference => reference.Name
            );
        }
    }
}
