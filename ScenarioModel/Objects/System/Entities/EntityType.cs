﻿using ScenarioModel.Objects.System;
using ScenarioModel.Objects.System.States;

namespace ScenarioModel.Objects.System.Entities;

/// <summary>
/// Types exist only to allow grouping and reuse of entities (that would then have the same state type and aspects)
/// </summary>
public record EntityType : IStatefulType, INameful
{
    public string Name { get; set; } = "";
    public StateMachine? StateMachine { get; set; }
    //public List<AspectType> AspectTypes { get; set; } = new();
}