﻿using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects.Interfaces;
using ScenarioModelling.Objects.SystemObjects.Properties;
using ScenarioModelling.Objects.Visitors;
using ScenarioModelling.References;
using ScenarioModelling.References.Interfaces;
using System.Text.Json.Serialization;

namespace ScenarioModelling.Objects.SystemObjects;

[ObjectLike<ISystemObject, Aspect>]
public record Aspect : ISystemObject<AspectReference>, IStateful, IRelatable
{
    private readonly System _system;

    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(Aspect);
    public AspectType? AspectType { get; set; } // TODO Propertyise
    public EntityProperty Entity { get; private init; }
    public RelationListProperty Relations { get; private init; }
    public StateProperty InitialState { get; private init; }
    public StateProperty State { get; private init; }

    public Aspect(System system)
    {
        _system = system;

        // Add this to the system
        system.Aspects.Add(this);

        Entity = new EntityProperty(system);
        InitialState = new StateProperty(system);
        State = new StateProperty(system);
        Relations = new RelationListProperty(system);
    }

    public AspectReference GenerateReference()
        => new AspectReference(_system) { Name = Name };

    public IStatefulObjectReference GenerateStatefulReference()
        => new AspectReference(_system) { Name = Name };

    IRelatableObjectReference ISystemObject<IRelatableObjectReference>.GenerateReference()
        => GenerateReference();

    IRelatableObjectReference IReferencable<IRelatableObjectReference>.GenerateReference()
        => GenerateReference();

    public object Accept(ISystemVisitor visitor)
        => visitor.VisitAspect(this);

}
