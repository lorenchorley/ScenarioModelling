using ScenarioModel.Collections;
using ScenarioModel.Execution.Events;
using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.ScenarioObjects;

public interface IScenarioNode : IDirectedGraphNode, INameful
{
}

public interface IScenarioNode<E> : IScenarioNode where E : IScenarioEvent
{
    public E GenerateEvent();
}
