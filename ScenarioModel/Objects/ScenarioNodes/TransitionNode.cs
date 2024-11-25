using ScenarioModel.Collections;
using ScenarioModel.Execution.Events;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Objects.ScenarioNodes.DataClasses;
using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.Objects.Visitors;
using ScenarioModel.References;
using ScenarioModel.References.Interfaces;

namespace ScenarioModel.Objects.ScenarioNodes;

[NodeLike<IScenarioNode, TransitionNode>]
public record TransitionNode : ScenarioNode<StateChangeEvent>
{
    [NodeLikeProperty(serialise: false)]
    public IStatefulObjectReference? StatefulObject { get; set; }

    [NodeLikeProperty(serialise: false)]
    public string TransitionName { get; set; } = "";

    public TransitionNode()
    {
        Name = "Transition";
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
            StatefulObject?.ResolveReference()
                          .Match(
                                Some: obj => obj,
                                None: () => throw new Exception("Stateful object not found")
                            )
                          ?? throw new Exception("StatefulObject was null");

        if (statefulObject.State == null)
        {
            if (statefulObject is IIdentifiable nameful)
                throw new Exception($"Attempted state transition {TransitionName} on {nameful.Name} but no state set initially");
            else
                throw new Exception($"Attempted state transition {TransitionName} on object but no state set initially");
        }

        var resolvedValue = statefulObject.State.ResolvedValue
                            ?? throw new Exception("Stateful object state is not set.");        

        e.InitialState = new StateReference(dependencies.Context.System) { Name = resolvedValue.Name };

        resolvedValue.DoTransition(TransitionName, statefulObject);

        resolvedValue = statefulObject.State.ResolvedValue
                        ?? throw new Exception("Stateful object state is not set.");

        e.FinalState = new StateReference(dependencies.Context.System) { Name = resolvedValue.Name };

        return e;
    }

    public override IEnumerable<SemiLinearSubGraph<IScenarioNode>> TargetSubgraphs()
        => Enumerable.Empty<SemiLinearSubGraph<IScenarioNode>>();

    public override OneOfIScenaroNode ToOneOf() => new OneOfIScenaroNode(this);

    public override object Accept(IScenarioVisitor visitor)
        => visitor.VisitTransitionNode(this);
}
