﻿using ScenarioModel.Objects.ScenarioObjects;

namespace ScenarioModel.Execution.Events;

public record JumpEvent : IScenarioEvent<JumpNode>
{
    public string Target { get; init; } = "";
    public JumpNode ProducerNode { get; init; } = null!;
}
