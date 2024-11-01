using ScenarioModel;
using ScenarioModel.Execution.Events;
using ScenarioModel.Objects.ScenarioObjects;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.Objects.SystemObjects.States;
using ScenarioModel.References;
using ScenarioModelling_ConsoleFront.NodeHandlers.BaseClasses;
using Spectre.Console;

namespace ScenarioModelling_ConsoleFront.NodeHandlers;

internal class StateTransitionNodeHandler : NodeHandler<StateTransitionNode, StateChangeEvent>
{
    public override void Handle(StateTransitionNode node, StateChangeEvent e)
    {
        
        AnsiConsole.Markup($"[white]{e.StatefulObject} : {e.InitialState} -> {e.FinalState} (Via transition : {e.TransitionName})[/]\n");
    }
}
