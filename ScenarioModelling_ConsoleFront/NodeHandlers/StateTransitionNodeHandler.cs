using ScenarioModel.Execution.Events;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModelling_ConsoleFront.NodeHandlers.BaseClasses;
using Spectre.Console;

namespace ScenarioModelling_ConsoleFront.NodeHandlers;

[NodeLike<INodeHandler, StateTransitionNode>]
internal class StateTransitionNodeHandler : NodeHandler<StateTransitionNode, StateChangeEvent>
{
    public override void Handle(StateTransitionNode node, StateChangeEvent e)
    {

        AnsiConsole.Markup($"[white]{e.StatefulObject} : {e.InitialState} -> {e.FinalState} (Via transition : {e.TransitionName})[/]\n");
    }
}
