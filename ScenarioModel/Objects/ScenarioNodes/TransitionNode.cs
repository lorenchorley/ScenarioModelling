using ScenarioModelling.Collections.Graph;
using ScenarioModelling.Execution.Events;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.ScenarioNodes.BaseClasses;
using ScenarioModelling.Objects.ScenarioNodes.DataClasses;
using ScenarioModelling.Objects.SystemObjects.Interfaces;
using ScenarioModelling.Objects.Visitors;
using ScenarioModelling.References;
using ScenarioModelling.References.Interfaces;
using System.Diagnostics;

namespace ScenarioModelling.Objects.ScenarioNodes;

[NodeLike<IScenarioNode, TransitionNode>]
public record TransitionNode : ScenarioNode<StateChangeEvent>
{
    [NodeLikeProperty(serialise: false)]
    public IStatefulObjectReference? StatefulObject { get; set; }

    [NodeLikeProperty(serialise: false)]
    public string TransitionName { get; set; } = "";

    public TransitionNode()
    {
    }

    public override StateChangeEvent GenerateEvent(EventGenerationDependencies dependencies)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(StatefulObject);

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

    [DebuggerNonUserCode]
    public override OneOfIScenaroNode ToOneOf() => new OneOfIScenaroNode(this);

    public override object Accept(IScenarioVisitor visitor)
        => visitor.VisitTransitionNode(this);

    public override bool IsFullyEqv(IScenarioNode other)
    {
        if (other is not TransitionNode otherNode)
            return false;

        if (!other.Name.IsEqv(Name))
            return false;

        if (!otherNode.TransitionName.IsEqv(TransitionName))
            return false;

        ArgumentNullExceptionStandard.ThrowIfNull(StatefulObject);
        ArgumentNullExceptionStandard.ThrowIfNull(otherNode.StatefulObject);

        if (!otherNode.StatefulObject.IsEqv(StatefulObject))
            return false;

        return true;
    }
}
