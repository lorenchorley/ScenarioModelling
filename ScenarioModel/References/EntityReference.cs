﻿using LanguageExt;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.References.Interfaces;

namespace ScenarioModel.References;

public record EntityReference(System System) : IReference<Entity>, IRelatableObjectReference, IStatefulObjectReference
{
    public string Name { get; set; } = "";
    public Type Type => typeof(Entity);

    public Option<Entity> ResolveReference()
        => System.Entities.Find(x => x.IsEqv(this));

    Option<IRelatable> IReference<IRelatable>.ResolveReference()
        => ResolveReference().Map(x => (IRelatable)x);

    Option<IStateful> IReference<IStateful>.ResolveReference()
        => ResolveReference().Map(x => (IStateful)x);

    public bool IsResolvable() => ResolveReference().IsSome;

    override public string ToString() => $"{Name}";

}
