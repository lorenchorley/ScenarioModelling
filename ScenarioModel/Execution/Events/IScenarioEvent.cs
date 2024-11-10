﻿using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.Execution.Events;

public interface IScenarioEvent
{

}

public interface IScenarioEvent<T> : IScenarioEvent
    where T : IScenarioNode
{
    T ProducerNode { get; }
}
