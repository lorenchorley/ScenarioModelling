using ScenarioModelling.CoreObjects.References;

namespace ScenarioModelling.CoreObjects.MetaStateObjects.Properties;

public class EntityProperty(MetaState System) : OptionalReferencableProperty<Entity, EntityReference>(System)
{
    public override string? Name
    {
        get
        {
            return _valueOrReference?.Match(
                entity => entity.Name,
                reference => reference.Name
            );
        }
    }
}
