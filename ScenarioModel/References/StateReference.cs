﻿using LanguageExt;
using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.References;

public record StateReference : IReference<State>
{
    public string StateName { get; set; } = "";

    public Option<State> ResolveReference(System system)
    {
        throw new NotImplementedException();
    }

    override public string ToString() => $"{StateName}";
}
