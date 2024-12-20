﻿using ScenarioModel.References;

namespace ScenarioModel.Objects.SystemObjects.Properties;

public class EntityProperty(System System) : OptionalReferencableProperty<Entity, EntityReference>(System)
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
