using ScenarioModel.Execution.Events;
using ScenarioModel.Objects.ScenarioObjects.BaseClasses;
using ScenarioModel.Objects.ScenarioObjects.DataClasses;

namespace ScenarioModelling_ConsoleFront.NodeHandlers.BaseClasses;
internal abstract class NodeHandler<T, E>
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
