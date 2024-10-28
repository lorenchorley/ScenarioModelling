using ScenarioModel.Collections;
using ScenarioModel.Execution.Events;
using ScenarioModel.Objects.System;

namespace ScenarioModel.Objects.Scenario;

public interface IScenarioNode : IDirectedGraphNode, INameful
{
}

public interface IScenarioNode<E> : IScenarioNode where E : IScenarioEvent
{
    public E GenerateEvent();
}
