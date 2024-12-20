﻿using ScenarioModel.Execution.Events.Interfaces;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Objects.ScenarioNodes.DataClasses;

namespace ScenarioModelling_ConsoleFront.NodeHandlers.BaseClasses;

internal interface INodeHandler
{
}

internal abstract class NodeHandler<T, E> : INodeHandler
    where T : ScenarioNode<E>
    where E : IScenarioEvent
{
    public EventGenerationDependencies Dependencies { get; init; } = null!;

    public void Manage(T node)
    {
        E e = node.GenerateEvent(Dependencies);

        Handle(node, e);

        Dependencies.Executor.RegisterEvent(e);
    }

    public abstract void Handle(T node, E e);
}
