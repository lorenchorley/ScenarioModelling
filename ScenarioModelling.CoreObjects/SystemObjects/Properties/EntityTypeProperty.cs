using ScenarioModelling.CoreObjects.References;

namespace ScenarioModelling.CoreObjects.SystemObjects.Properties;

public class EntityTypeProperty(MetaState System) : OptionalReferencableProperty<EntityType, EntityTypeReference>(System)
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
