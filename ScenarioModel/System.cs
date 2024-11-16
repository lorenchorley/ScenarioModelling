using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.References;
using ScenarioModel.References.Interfaces;

namespace ScenarioModel;

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

    public void ValidateAndInitialise()
    {
        CheckForUnresolvableReferences();
        CheckForNameUniquenessByType();
    }

    #region Validation
    public void CheckForUnresolvableReferences()
    {
        List<string> unresolvedReferenceDescriptions =
        [
            .. CheckReferenceResolvability<Entity>(AllEntityReferences, name => $"Entity reference : {name}"),
            .. CheckReferenceResolvability<EntityType>(AllEntityTypeReferences, name => $"Entity type reference : {name}"),
            .. CheckReferenceResolvability<State>(AllStateReferences, name => $"State reference : {name}"),
            .. CheckReferenceResolvability<Relation>(AllRelationReferences, name => $"Relation reference : {name}"),
            .. CheckReferenceResolvability<Aspect>(AllAspectReferences, name => $"Aspect reference : {name}"),
            .. CheckReferenceResolvability<StateMachine>(AllStateMachineReferences, name => $"StateMachine reference : {name}"),
            .. CheckReferenceResolvability<Transition>(AllTransitionReferences, name => $"Transition reference : {name}"),
            .. CheckReferenceResolvability<Constraint>(AllConstraintReferences, name => $"Constraint reference : {name}")
        ];

        if (unresolvedReferenceDescriptions.Any())
        {
            throw new Exception($"Unresolved references found : {unresolvedReferenceDescriptions.BulletPointList()}");
        }
    }

    private IEnumerable<string> CheckReferenceResolvability<TValue>(IEnumerable<IReference<TValue>> references, Func<IReference<TValue>, string> toMessage)
    {
        foreach (var reference in references)
        {
            if (reference.ResolveReference().IsNone)
            {
                yield return toMessage(reference);
            }
        }
    }

    public void CheckForNameUniquenessByType()
    {
        List<string> unresolvedReferenceDescriptions =
        [
            .. CheckNameUniqueness<Entity>(Entities),
            .. CheckNameUniqueness<EntityType>(EntityTypes),
            .. CheckNameUniqueness<State>(States),
            .. CheckNameUniqueness<Relation>(Relations),
            .. CheckNameUniqueness<Aspect>(Aspects),
            .. CheckNameUniqueness<StateMachine>(StateMachines),
            //.. CheckNameUniqueness<Transition>(Transitions) // It's ok to have duplicate transition names, it's wanted even
        ];

        if (unresolvedReferenceDescriptions.Any())
        {
            throw new Exception($"Name uniqueness by object type not satisfied : {unresolvedReferenceDescriptions.BulletPointList()}");
        }
    }

    private IEnumerable<string> CheckNameUniqueness<TValue>(IEnumerable<IIdentifiable> values)
    {
        foreach (var groupsWithMoreThanOneName in values.GroupBy(v => v.Name).Where(g => g.Count() > 1))
        {
            yield return $"Found more than one {typeof(TValue).Name} with name {groupsWithMoreThanOneName.Key} ({groupsWithMoreThanOneName.Count()})";
        }
    }
    #endregion

    public IEnumerable<EntityReference> AllEntityReferences
    {
        get => Enumerable.Empty<EntityReference>()
                         .Concat(Aspects.Select(e => e.Entity.Reference))
                         .Where(s => s != null)
                         .Cast<EntityReference>();
    }

    public IEnumerable<EntityTypeReference> AllEntityTypeReferences
    {
        get => Enumerable.Empty<EntityTypeReference>()
                         .Concat(Entities.Select(e => e.EntityType.Reference))
                         .Where(s => s != null)
                         .Cast<EntityTypeReference>();
    }

    public IEnumerable<AspectReference> AllAspectReferences
    {
        get => Enumerable.Empty<AspectReference>()
                         .Concat(Entities.SelectMany(e => e.Aspects.AllReferences))
                         .Where(s => s != null)
                         .Cast<AspectReference>();
    }

    public IEnumerable<StateReference> AllStateReferences
    {
        get => AllStateful.Select(e => e.State.Reference)
                          .Concat(StateMachines.SelectMany(sm => sm.States.AllReferences))
                          .Append(Transitions.Select(t => t.SourceState.Reference))
                          .Append(Transitions.Select(t => t.DestinationState.Reference))
                          .Where(s => s != null)
                          .Cast<StateReference>();
    }

    public IEnumerable<StateMachineReference> AllStateMachineReferences
    {
        get => Enumerable.Empty<StateMachineReference>()
                         .Concat(Aspects.Select(e => e.AspectType?.StateMachine.Reference))
                         .Concat(EntityTypes.Select(e => e.StateMachine.Reference))
                         .Where(s => s != null)
                         .Cast<StateMachineReference>();
    }

    public IEnumerable<RelationReference> AllRelationReferences
    {
        get => Enumerable.Empty<RelationReference>()
                         .Concat(AllRelatable.SelectMany(e => e.Relations.AllReferences))
                         .Where(s => s != null)
                         .Cast<RelationReference>();
    }

    public IEnumerable<TransitionReference> AllTransitionReferences
    {
        get => Enumerable.Empty<TransitionReference>()
                         .Concat(StateMachines.SelectMany(s => s.Transitions.AllReferences))
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
