using ScenarioModel;
using ScenarioModel.Execution.Dialog;
using ScenarioModel.Execution.Events;
using ScenarioModel.Interpolation;
using ScenarioModel.Objects.Scenario;

namespace ScenarioModelling_ConsoleFront.NodeHandlers;
internal abstract class NodeHandler<T, E> 
    where T : IScenarioNode<E> 
    where E : IScenarioEvent, new()
{
    public StringInterpolator Interpolator { get; init; } = null!;
    public DialogExecutor DialogFactory { get; init; } = null!;
    public Context Context { get; init; } = null!;

    public void Manage(T node)
    {
        E e = node.GenerateEvent();

        Handle(node, e);

        DialogFactory.RegisterEvent(e);
    }

    public abstract void Handle(T node, E e);
}
