using LanguageExt;
using ScenarioModel.ContextValidation.Errors;
using ScenarioModel.ContextValidation.Interfaces;
using ScenarioModel.ContextValidation.SystemValidation;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.Objects.Visitors;
using ScenarioModel.References.Interfaces;
using Relation = ScenarioModel.Objects.SystemObjects.Relation;

namespace ScenarioModel.ContextValidation;

public class SystemValidator : ISystemVisitor
{
    private readonly AspectValidator _aspectValidator = new();
    private readonly ConstraintValidator _constraintValidator = new();
    private readonly EntityValidator _entityValidator = new();
    private readonly EntityTypeValidator _entityTypeValidator = new();
    private readonly RelationValidator _relationValidator = new();
    private readonly StateValidator _stateValidator = new();
    private readonly StateMachineValidator _stateMachineValidator = new();
    private readonly TransitionValidator _transitionValidator = new();

    private System? _system;

    public ValidationErrors Validate(System system)
    {
        SystemObjectExhaustivity.AssertInterfaceExhaustivelyImplemented<IObjectValidator>();

        _system = system;

        ValidationErrors validationErrors = new();

        // All names are unique
        // Everything is valid

        //ValidateEntityTypes(system, validationErrors);
        //ValidateStateMachines(system, validationErrors);
        //ValidateStates(system, validationErrors);
        //ValidateRelations(system, validationErrors);
        //ValidateConstraints(system, validationErrors);

        CheckForUnresolvableReferences(system);
        CheckForNameUniquenessByType(system);

        _system = null;

        return validationErrors;
    }

    private void ValidateRelations(System system, ValidationErrors validationErrors)
    {
        IEnumerable<Relation> relations =
            Enumerable.Empty<Relation>()
                .Concat(system.Entities.SelectMany(e => e.Relations))
                .Concat(system.Entities.SelectMany(e => e.Relations))
                .Distinct();

        foreach (var relation in relations)
        {
            _relationValidator.Validate(_system!, relation);
        }
    }

    private void ValidateStateMachines(System system, ValidationErrors validationErrors)
    {
        // Uniqueness of state type names
        var names = system.StateMachines.GroupBy(s => s.Name);
        foreach (var name in names)
        {
            validationErrors.AddIf(name.Count() > 1, new NameNotUnique($"State type name {name.Key} is not unique. {name.Count()} instances found."));
        }
    }

    private void ValidateStates(System system, ValidationErrors validationErrors)
    {
        //// Uniqueness of state names
        //var names = system.States.GroupBy(s => s.Name);
        //foreach (var name in names)
        //{
        //    // Determine if there are multiple instances of the class in this list
        //    int uniqueInstances = name.UniqueObjectInstanceCount();

        //    //var uniqueReferencesToName = name.ToList().DistinctBy(s => s.StateMachine);
        //    validationErrors.AddIf(uniqueInstances > 1, new NameNotUnique($"State name {name.Key} is not unique. {name.Count()} instances found."));
        //}

        foreach (var state in system.States)
        {

        }
    }

    private void CheckForUnresolvableReferences(System system)
    {
        List<string> unresolvedReferenceDescriptions =
        [
            .. CheckReferenceResolvability<Entity>(system.AllEntityReferences, name => $"Entity reference : {name}"),
            .. CheckReferenceResolvability<EntityType>(system.AllEntityTypeReferences, name => $"Entity type reference : {name}"),
            .. CheckReferenceResolvability<State>(system.AllStateReferences, name => $"State reference : {name}"),
            .. CheckReferenceResolvability<Relation>(system.AllRelationReferences, name => $"Relation reference : {name}"),
            .. CheckReferenceResolvability<Aspect>(system.AllAspectReferences, name => $"Aspect reference : {name}"),
            .. CheckReferenceResolvability<StateMachine>(system.AllStateMachineReferences, name => $"StateMachine reference : {name}"),
            .. CheckReferenceResolvability<Transition>(system.AllTransitionReferences, name => $"Transition reference : {name}"),
            .. CheckReferenceResolvability<Constraint>(system.AllConstraintReferences, name => $"Constraint reference : {name}")
        ];

        if (unresolvedReferenceDescriptions.Any())
        {
            throw new Exception($"Unresolved references found : {unresolvedReferenceDescriptions.BulletPointList()}");
        }
    }

    private IEnumerable<string> CheckReferenceResolvability<TValue>(IEnumerable<IReference> references, Func<IReference, string> toMessage)
    {
        foreach (var reference in references)
        {
            if (!reference.Resolve<TValue>().Any())
            {
                yield return toMessage(reference);
            }
        }
    }

    private void CheckForNameUniquenessByType(System system)
    {
        List<string> unresolvedReferenceDescriptions =
        [
            .. CheckNameUniqueness<Entity>(system.Entities),
            .. CheckNameUniqueness<EntityType>(system.EntityTypes),
            .. CheckNameUniqueness<State>(system.States),
            .. CheckNameUniqueness<Relation>(system.Relations),
            .. CheckNameUniqueness<Aspect>(system.Aspects),
            .. CheckNameUniqueness<StateMachine>(system.StateMachines),
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

    public object VisitAspect(Aspect aspect)
    {
        ArgumentNullException.ThrowIfNull(_system);
        return _aspectValidator.Validate(_system, aspect);
    }

    public object VisitAspectType(AspectType aspectType)
    {
        throw new NotImplementedException();
        //ArgumentNullException.ThrowIfNull(_system);
        //return _aspectTypeValidator.Validate(_system, aspectType);
    }

    public object VisitConstraint(Constraint constraint)
    {
        ArgumentNullException.ThrowIfNull(_system);
        return _constraintValidator.Validate(_system, constraint);
    }

    public object VisitEntity(Entity entity)
    {
        ArgumentNullException.ThrowIfNull(_system);
        return _entityValidator.Validate(_system, entity);
    }

    public object VisitEntityType(EntityType entityType)
    {
        ArgumentNullException.ThrowIfNull(_system);
        return _entityTypeValidator.Validate(_system, entityType);
    }

    public object VisitRelation(Relation relation)
    {
        ArgumentNullException.ThrowIfNull(_system);
        return _relationValidator.Validate(_system, relation);
    }

    public object VisitState(State state)
    {
        ArgumentNullException.ThrowIfNull(_system);
        return _stateValidator.Validate(_system, state);
    }

    public object VisitStateMachine(StateMachine stateMachine)
    {
        ArgumentNullException.ThrowIfNull(_system);
        return _stateMachineValidator.Validate(_system, stateMachine);
    }

    public object VisitTransition(Transition transition)
    {
        ArgumentNullException.ThrowIfNull(_system);
        return _transitionValidator.Validate(_system, transition);
    }
}