﻿using LanguageExt;
using ScenarioModel.Objects.System.Entities;
using ScenarioModel.Objects.System.Relations;
using ScenarioModel.Objects.System.States;

namespace ScenarioModel.References;

public record EntityAspectReference : IReference<Aspect>, IRelatableObjectReference, IStatefulObjectReference
{
    public string EntityName { get; set; } = "";
    public string AspectName { get; set; } = "";

    public Option<Aspect> ResolveReference(System system)
    {
        throw new NotImplementedException();
    }

    Option<IStateful> IReference<IStateful>.ResolveReference(System system)
        => ResolveReference(system).Map(x => (IStateful)x);

    Option<IRelatable> IReference<IRelatable>.ResolveReference(System system)
        => ResolveReference(system).Map(x => (IRelatable)x);

    override public string ToString() => $"{EntityName}.{AspectName}";
}
