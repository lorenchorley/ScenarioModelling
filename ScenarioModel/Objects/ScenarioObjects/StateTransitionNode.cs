﻿using ScenarioModel.Collections;
using ScenarioModel.Execution.Events;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.ScenarioObjects.BaseClasses;
using ScenarioModel.Objects.ScenarioObjects.DataClasses;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.Objects.SystemObjects.States;
using ScenarioModel.References;

namespace ScenarioModel.Objects.ScenarioObjects;

[NodeLike<IScenarioNode, StateTransitionNode>]
public record StateTransitionNode : ScenarioNode<StateChangeEvent>
{
    [NodeLikeProperty(serialise: false)]
    public IStatefulObjectReference? StatefulObject { get; set; }

    [NodeLikeProperty(serialise: false)]
    public string TransitionName { get; set; } = "";

    public StateTransitionNode()
    {
        Name = "StateTransition";
    }

    public override StateChangeEvent GenerateEvent(EventGenerationDependencies dependencies)
    {
        ArgumentNullException.ThrowIfNull(StatefulObject);

        StateChangeEvent e = new StateChangeEvent()
        {
            ProducerNode = this,
            StatefulObject = StatefulObject,
            TransitionName = TransitionName
        };

        IStateful statefulObject =
            StatefulObject?.ResolveReference(dependencies.Context.System)
                          .Match(
                                Some: obj => obj,
                                None: () => throw new Exception("Stateful object not found")
                            )
                          ?? throw new Exception("StatefulObject was null");

        if (statefulObject.State == null)
        {
            if (statefulObject is INameful nameful)
            {
                throw new Exception($"Attempted state transition {TransitionName} on {nameful.Name} but no state set initially");
            }
            else
            {
                throw new Exception($"Attempted state transition {TransitionName} on object but no state set initially");
            }
        }

        e.InitialState = new StateReference() { StateName = statefulObject.State.ResolvedValue.Name };

        if (!statefulObject.State.ResolvedValue.TryTransition(TransitionName, statefulObject))
        {
            throw new Exception($"State transition failed, no such transition {TransitionName} on state {statefulObject.State.ResolvedValue.Name} of type {statefulObject.State.ResolvedValue.StateMachine.Name}");
        }

        e.FinalState = new StateReference() { StateName = statefulObject.State.ResolvedValue.Name };

        return e;
    }

    public override IEnumerable<SemiLinearSubGraph<IScenarioNode>> TargetSubgraphs()
        => Enumerable.Empty<SemiLinearSubGraph<IScenarioNode>>();

    public override OneOfIScenaroNode ToOneOf() => new OneOfIScenaroNode(this);
}
