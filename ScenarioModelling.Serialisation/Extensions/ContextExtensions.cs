using ScenarioModelling.CoreObjects;
using ScenarioModelling.Exhaustiveness;

public static class ContextExtensions
{
    public static Context Incorporate(this Context context, Context other)
    {
        context.MetaStories.AddRange(other.MetaStories); // TODO Merge existing meta stories with the same name, or throw on inconsistency

        SystemObjectExhaustivity.DoForEachObjectType(
            entity: () => context.MetaState.Entities.AddRange(other.MetaState.Entities),
            entityType: () => context.MetaState.EntityTypes.AddRange(other.MetaState.EntityTypes),
            aspect: () => context.MetaState.Aspects.AddRange(other.MetaState.Aspects),
            relation: () => context.MetaState.Relations.AddRange(other.MetaState.Relations),
            state: () => context.MetaState.States.AddRange(other.MetaState.States),
            stateMachine: () => context.MetaState.StateMachines.AddRange(other.MetaState.StateMachines),
            transition: () => context.MetaState.Transitions.AddRange(other.MetaState.Transitions),
            constraint: () => context.MetaState.Constraints.AddRange(other.MetaState.Constraints)
        );

        return context;
    }
}
