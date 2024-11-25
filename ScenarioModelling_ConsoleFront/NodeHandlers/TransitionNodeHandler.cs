using ScenarioModel.Execution.Events;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModelling_ConsoleFront.NodeHandlers.BaseClasses;
using Spectre.Console;

namespace ScenarioModelling_ConsoleFront.NodeHandlers;

[NodeLike<INodeHandler, TransitionNode>]
internal class TransitionNodeHandler : NodeHandler<TransitionNode, StateChangeEvent>
{
    public override void Handle(TransitionNode node, StateChangeEvent e)
    {

        AnsiConsole.Markup($"[white]{e.StatefulObject} : {e.InitialState} -> {e.FinalState} (Via transition : {e.TransitionName})[/]\n");
    }
}
