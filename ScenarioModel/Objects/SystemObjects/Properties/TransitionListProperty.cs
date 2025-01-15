using ScenarioModelling.References;

namespace ScenarioModelling.Objects.SystemObjects.Properties;

public class TransitionListProperty : ReferencableSetProperty<Transition, TransitionReference>
{
    public TransitionListProperty(System system) : base(system, new TransitionEquivalanceComparer(system))
    {

    }

    private class TransitionEquivalanceComparer(System System) : OneOfValOrRefEquivalanceComparer<Transition, TransitionReference>
    {
        protected override bool AreEqual(TransitionReference rx, TransitionReference ry)
            => rx.Equals(ry);

        protected override bool AreEqual(Transition vx, Transition vy)
            => vx.Equals(vy);
    }
}
