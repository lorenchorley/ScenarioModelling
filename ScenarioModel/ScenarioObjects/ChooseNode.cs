﻿using ScenarioModel.Execution.Events;

namespace ScenarioModel.ScenarioObjects;

public record ChooseNode : IScenarioNode<ChoiceSelectedEvent>
{
    public string Name { get; set; } = "";
    public ChoiceList Choices { get; set; } = new();

    public IEnumerable<string> TargetNodeNames => Choices;

    public ChoiceSelectedEvent GenerateEvent()
    {
        return new ChoiceSelectedEvent() { ProducerNode = this };
    }
}