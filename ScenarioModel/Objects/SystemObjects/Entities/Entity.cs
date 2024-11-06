using ScenarioModel.Objects.SystemObjects.Properties;
using ScenarioModel.Objects.SystemObjects.Relations;
using ScenarioModel.Objects.SystemObjects.States;
using ScenarioModel.References;

namespace ScenarioModel.Objects.SystemObjects.Entities;

public record Entity : IStateful, IRelatable, INameful
{
    public string Name { get; set; } = "";
    public EntityType EntityType { get; set; } = null!;
    public List<Relation> Relations { get; set; } = new();
    public List<Aspect> Aspects { get; set; } = new();
    public string CharacterStyle { get; set; } = "";
    public StateProperty State { get; private init; }

    public Entity(System system)
    {
        // Add this entity to the system
        system.Entities.Add(this);

        State = new StateProperty(system);
    }

    public IStatefulObjectReference GenerateReference()
    {
        return new EntityReference() { EntityName = Name };
    }

    public void AssertEqv(Entity other)
    {
        if (!Name.IsEqv(other.Name))
            throw new Exception($"Entity names do not match: '{Name}' and '{other.Name}'.");

        //if (!EntityType.Name.IsEqv(other.Name))
        //    throw new Exception($"Entity types do not match: '{EntityType.Name}' and '{other.EntityType.Name}'.");

        if (Relations.Count != other.Relations.Count)
            throw new Exception($"Entity '{Name}' has {Relations.Count} relations, but entity '{other.Name}' has {other.Relations.Count}.");

        // There must be an equivalent relation for each relation, not complete but good enough perhaps
        foreach (var relation in Relations)
        {
            if (!other.Relations.Any(r => r.IsEqv(relation)))
            {
                throw new Exception($"No equivalent relation '{relation.Name}' not found in entity '{other.Name}'.");
            }
        }

        // There must be an equivalent aspect for each aspect, not complete but good enough perhaps
        foreach (var aspect in Aspects)
        {
            if (!other.Aspects.Any(a => a.IsEqv(aspect)))
            {
                throw new Exception($"No equivalent aspect '{aspect.Name}' not found in entity '{other.Name}'.");
            }
        }

        if (!CharacterStyle.IsEqv(other.CharacterStyle))
        {
            throw new Exception($"Character styles do not match: '{CharacterStyle}' and '{other.CharacterStyle}'.");
        }

        if (!State.IsEqv(other.State))
        {
            throw new Exception($"States do not match: '{State}' and '{other.State}'.");
        }
    }
}
