using ScenarioModel;
using ScenarioModel.Execution.Events;
using ScenarioModel.Objects.Scenario;
using ScenarioModel.Objects.System;
using ScenarioModel.Objects.System.States;
using ScenarioModel.References;
using Spectre.Console;

namespace ScenarioModelling_ConsoleFront.NodeHandlers;

internal class StateTransitionNodeHandler : NodeHandler<StateTransitionNode, StateChangeEvent>
{
    public override void Handle(StateTransitionNode node, StateChangeEvent e)
    {
        IStateful statefulObject =
            node.StatefulObject
                          ?.ResolveReference(Context.System)
                          .Match(
                                Some: obj => obj,
                                None: () => throw new Exception("Stateful object not found")
                            )
                          ?? throw new Exception("StatefulObject was null");

        if (statefulObject.State == null)
        {
            if (statefulObject is INameful nameful)
            {
                throw new Exception($"Attempted state transition {node.TransitionName} on {nameful.Name} but no state set initially");
            }
            else
            {
                throw new Exception($"Attempted state transition {node.TransitionName} on object but no state set initially");
            }
        }

        e.InitialState = new StateReference() { StateName = statefulObject.State.Name };

        if (!statefulObject.State.TryTransition(node.TransitionName, statefulObject))
        {
            throw new Exception($"State transition failed, no such transition {node.TransitionName} on state {statefulObject.State.Name} of type {statefulObject.State.StateMachine.Name}");
        }

        e.FinalState = new StateReference() { StateName = statefulObject.State.Name };

        AnsiConsole.Markup($"[white]{e.StatefulObject} : {e.InitialState} -> {e.FinalState} (Via transition : {e.TransitionName})[/]\n");
    }
}
