﻿using ScenarioModel.References;
using ScenarioModel.SystemObjects.Entities;

namespace ScenarioModel.ScenarioObjects.Events;

public class AddRelationEvent : IScenarioEvent
{
    public IReference First { get; set; } = null!;
    public IReference Second { get; set; } = null!;
    public IScenarioNode ProducerNode { get; init; } = null!;
}
