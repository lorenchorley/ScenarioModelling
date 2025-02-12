using ScenarioModelling.CoreObjects.SystemObjects;

namespace ScenarioModelling.CoreObjects.Visitors;

public interface ISystemVisitor
{
    object VisitAspect(Aspect aspect);
    object VisitAspectType(AspectType aspectType);
    object VisitConstraint(Constraint constraint);
    object VisitEntity(Entity entity);
    object VisitEntityType(EntityType entityType);
    object VisitRelation(Relation relation);
    object VisitState(State state);
    object VisitStateMachine(StateMachine stateMachine);
    object VisitTransition(Transition transition);
}
