using OneOf;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using System.Diagnostics;

namespace ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;

public class OneOfMetaStateObject : OneOfBase<Aspect, Constraint, Entity, EntityType, Relation, State, StateMachine, Transition>
{
    [DebuggerNonUserCode]
    public OneOfMetaStateObject(OneOf<Aspect, Constraint, Entity, EntityType, Relation, State, StateMachine, Transition> input) : base(input)
    {

    }
}
