using ScenarioModelling.Expressions.SemanticTree;
using ScenarioModelling.Objects.SystemObjects;
using ScenarioModelling.Objects.SystemObjects.Interfaces;
using ScenarioModelling.References;

namespace ScenarioModelling;

public class System
{
    public List<Entity> Entities { get; set; } = new();
    public List<EntityType> EntityTypes { get; set; } = new();
    public List<Aspect> Aspects { get; set; } = new();
    public List<State> States { get; set; } = new();
    public List<StateMachine> StateMachines { get; set; } = new();
    public List<Relation> Relations { get; set; } = new();
    public List<Transition> Transitions { get; set; } = new();
    public List<Constraint> Constraints { get; set; } = new();

    public IEnumerable<EntityReference> AllEntityReferences
    {
        get => Enumerable.Empty<EntityReference>()
                         .Concat(Aspects.Select(e => e.Entity.ReferenceOnly))
                         .Where(s => s != null)
                         .Cast<EntityReference>();
    }

    public IEnumerable<EntityTypeReference> AllEntityTypeReferences
    {
        get => Enumerable.Empty<EntityTypeReference>()
                         .Concat(Entities.Select(e => e.EntityType.ReferenceOnly))
                         .Where(s => s != null)
                         .Cast<EntityTypeReference>();
    }

    public IEnumerable<AspectReference> AllAspectReferences
    {
        get => Enumerable.Empty<AspectReference>()
                         .Concat(Entities.SelectMany(e => e.Aspects.AllReferencesOnly))
                         .Where(s => s != null)
                         .Cast<AspectReference>();
    }

    public IEnumerable<StateReference> AllStateReferences
    {
        get => AllStateful.Select(e => e.State.ReferenceOnly)
                          .Concat(StateMachines.SelectMany(sm => sm.States.AllReferencesOnly))
                          .Append(Transitions.Select(t => t.SourceState.ReferenceOnly))
                          .Append(Transitions.Select(t => t.DestinationState.ReferenceOnly))
                          .Where(s => s != null)
                          .Cast<StateReference>();
    }

    public IEnumerable<StateMachineReference> AllStateMachineReferences
    {
        get => Enumerable.Empty<StateMachineReference>()
                         .Concat(Aspects.Select(e => e.AspectType?.StateMachine.ReferenceOnly))
                         .Concat(EntityTypes.Select(e => e.StateMachine.ReferenceOnly))
                         .Where(s => s != null)
                         .Cast<StateMachineReference>();
    }

    public IEnumerable<RelationReference> AllRelationReferences
    {
        get => Enumerable.Empty<RelationReference>()
                         .Concat(AllRelatable.SelectMany(e => e.Relations.AllReferencesOnly))
                         .Where(s => s != null)
                         .Cast<RelationReference>();
    }

    public IEnumerable<TransitionReference> AllTransitionReferences
    {
        get => Enumerable.Empty<TransitionReference>()
                         .Concat(StateMachines.SelectMany(s => s.Transitions.AllReferencesOnly))
                         .Where(s => s != null)
                         .Cast<TransitionReference>();
    }

    public IEnumerable<IStateful> AllStateful
    {
        get => Enumerable.Empty<IStateful>()
                         .Concat(Entities)
                         .Concat(AllAspects)
                         .Concat(Relations);
    }

    public IEnumerable<IRelatable> AllRelatable
    {
        get => Enumerable.Empty<IRelatable>()
                         .Concat(Entities)
                         .Concat(Entities.SelectMany(x => x.Aspects));
    }

    public IEnumerable<Aspect> AllAspects
    {
        get => Enumerable.Empty<Aspect>()
                         .Concat(Entities.SelectMany(x => x.Aspects));
    }

    public IEnumerable<ConstraintReference> AllConstraintReferences
    {
        get => Enumerable.Empty<ConstraintReference>();
        //.Concat(Constraints.SelectMany(x => x.Aspects));
    }

    /// <summary>
    /// Not finished yet, not all cases covered !
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    internal object ResolveValue(CompositeValue value)
    {
        if (value.ValueList.Count == 0)
        {
            throw new Exception("Value cannot be empty");
        }

        string first = value.ValueList[0];

        Entity? entity = Entities.FirstOrDefault(e => e.Name.IsEqv(first));

        if (entity != null)
        {
            if (value.ValueList.Count == 1)
            {
                return entity;
            }

            if (value.ValueList[1].IsEqv("State"))
            {
                return entity.State.ResolvedValue.Name;
            }

            // TODO aspects and other cases
        }

        // TODO other cases

        if (value.ValueList.Count == 1)
        {
            // we can suppose it's just a string
            return value.ValueList[0];
        }

        throw new Exception("Unsupported value");
    }
}
