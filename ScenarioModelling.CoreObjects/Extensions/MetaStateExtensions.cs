using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;
using ScenarioModelling.CoreObjects.References;

public static class MetaStateExtensions
{
    extension(MetaState metaState) 
    {
        public IEnumerable<EntityReference> AllEntityReferences
        => Enumerable.Empty<EntityReference>()
                     .Concat(metaState.Aspects.Select(e => e.Entity.ReferenceOnly))
                     .Where(s => s != null)
                     .Cast<EntityReference>();

        public IEnumerable<EntityTypeReference> AllEntityTypeReferences
            => Enumerable.Empty<EntityTypeReference>()
                         .Concat(metaState.Entities.Select(e => e.EntityType.ReferenceOnly))
                         .Where(s => s != null)
                         .Cast<EntityTypeReference>();

        public IEnumerable<AspectReference> AllAspectReferences
            => Enumerable.Empty<AspectReference>()
                         .Concat(metaState.Entities.SelectMany(e => e.Aspects.AllReferencesOnly))
                         .Where(s => s != null)
                         .Cast<AspectReference>();

        public IEnumerable<StateReference> AllStateReferences
            => metaState.AllStateful
                        .Select(e => e.State.ReferenceOnly)
                        .Concat(metaState.StateMachines.SelectMany(sm => sm.States.AllReferencesOnly))
                        .Append(metaState.Transitions.Select(t => t.SourceState.ReferenceOnly))
                        .Append(metaState.Transitions.Select(t => t.DestinationState.ReferenceOnly))
                        .Where(s => s != null)
                        .Cast<StateReference>();

        public IEnumerable<StateMachineReference> AllStateMachineReferences
            => Enumerable.Empty<StateMachineReference>()
                         .Concat(metaState.Aspects.Select(e => e.AspectType?.StateMachine.ReferenceOnly))
                         .Concat(metaState.EntityTypes.Select(e => e.StateMachine.ReferenceOnly))
                         .Where(s => s != null)
                         .Cast<StateMachineReference>();

        public IEnumerable<RelationReference> AllRelationReferences
            => Enumerable.Empty<RelationReference>()
                         .Concat(metaState.AllRelatable.SelectMany(e => e.Relations.AllReferencesOnly))
                         .Where(s => s != null)
                         .Cast<RelationReference>();

        public IEnumerable<TransitionReference> AllTransitionReferences
            => Enumerable.Empty<TransitionReference>()
                         .Concat(metaState.StateMachines.SelectMany(s => s.Transitions.AllReferencesOnly))
                         .Where(s => s != null)
                         .Cast<TransitionReference>();

        public IEnumerable<IStateful> AllStateful
            => Enumerable.Empty<IStateful>()
                         .Concat(metaState.Entities)
                         .Concat(metaState.AllAspects)
                         .Concat(metaState.Relations);

        public IEnumerable<IRelatable> AllRelatable
            => Enumerable.Empty<IRelatable>()
                         .Concat(metaState.Entities)
                         .Concat(metaState.Entities.SelectMany(x => x.Aspects));

        public IEnumerable<Aspect> AllAspects
            => Enumerable.Empty<Aspect>()
                         .Concat(metaState.Entities.SelectMany(x => x.Aspects));

        public IEnumerable<ConstraintReference> AllConstraintReferences
            => Enumerable.Empty<ConstraintReference>();
    }
}
