using Newtonsoft.Json;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects.Interfaces;
using ScenarioModelling.Objects.SystemObjects.Properties;
using ScenarioModelling.Objects.Visitors;
using ScenarioModelling.References;
using ScenarioModelling.References.Interfaces;

namespace ScenarioModelling.Objects.SystemObjects;

[ObjectLike<ISystemObject, Entity>]
public record Entity : ISystemObject<EntityReference>, IStateful, IRelatable
{
    private readonly System _system;

    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(Entity);

    public string CharacterStyle { get; set; } = "";
    public EntityTypeProperty EntityType { get; private set; }
    public RelationListProperty Relations { get; private set; }
    public AspectListProperty Aspects { get; private set; }
    public StateProperty InitialState { get; private set; }
    public StateProperty State { get; private set; }

    public Entity(System system)
    {
        _system = system;

        // Add this to the system
        system.Entities.Add(this);

        InitialState = new StateProperty(system);
        State = new StateProperty(system);
        Relations = new RelationListProperty(system);
        Aspects = new AspectListProperty(system);
        EntityType = new EntityTypeProperty(system);
    }

    public EntityReference GenerateReference()
        => new EntityReference(_system) { Name = Name };

    public IStatefulObjectReference GenerateStatefulReference()
        => GenerateReference();

    IRelatableObjectReference ISystemObject<IRelatableObjectReference>.GenerateReference()
        => GenerateReference();

    IRelatableObjectReference IReferencable<IRelatableObjectReference>.GenerateReference()
        => GenerateReference();

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

    public object Accept(ISystemVisitor visitor)
        => visitor.VisitEntity(this);
}
