﻿using Newtonsoft.Json;
using ProtoBuf;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects.Properties;
using ScenarioModelling.CoreObjects.Visitors;
using YamlDotNet.Serialization;

namespace ScenarioModelling.CoreObjects.MetaStateObjects;

[ProtoContract]
[MetaStateObjectLike<ISystemObject, Entity>]
public record Entity : ISystemObject<EntityReference>, IStateful, IRelatable
{
    private MetaState _system = null!;

    [JsonIgnore]
    [YamlIgnore]
    public Type Type => typeof(Entity);

    [ProtoMember(1)]
    public string Name { get; set; } = "";

    [ProtoMember(2)]
    public string CharacterStyle { get; set; } = "";

    [ProtoMember(3)]
    public EntityTypeProperty EntityType { get; private set; }

    [ProtoMember(4)]
    public RelationListProperty Relations { get; private set; }

    [ProtoMember(5)]
    public AspectListProperty Aspects { get; private set; }

    [ProtoMember(6)]
    public StateProperty InitialState { get; private set; }

    [ProtoMember(7)]
    public StateProperty State { get; private set; }

    private Entity()
    {

    }

    public Entity(MetaState system)
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

    public void InitialiseAfterDeserialisation(MetaState system)
    {
        _system = system;
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

    public object Accept(IMetaStateVisitor visitor)
        => visitor.VisitEntity(this);
}
