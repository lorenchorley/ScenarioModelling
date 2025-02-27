using ScenarioModelling.CoreObjects.References;

namespace ScenarioModelling.CoreObjects.MetaStateObjects.Properties;

public class TransitionListProperty : ReferencableSetProperty<Transition, TransitionReference>
{
    public TransitionListProperty(MetaState system) : base(system, new TransitionEquivalanceComparer(system))
    {

    }

    private class TransitionEquivalanceComparer(MetaState system) : OneOfValOrRefEquivalanceComparer<Transition, TransitionReference>
    {
        protected override bool AreEqual(TransitionReference rx, TransitionReference ry)
            => rx.Equals(ry);

        protected override bool AreEqual(Transition vx, Transition vy)
            => vx.Equals(vy);
    }
}
