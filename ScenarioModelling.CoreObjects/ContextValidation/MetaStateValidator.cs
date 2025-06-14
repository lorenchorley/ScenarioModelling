﻿using LanguageExt;
using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.ContextValidation.MetaStateValidation;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.CoreObjects.Visitors;
using Relation = ScenarioModelling.CoreObjects.MetaStateObjects.Relation;
using ScenarioModelling.CoreObjects.References;

namespace ScenarioModelling.CoreObjects.ContextValidation;

public class MetaStateValidator : IMetaStateVisitor
{
    private readonly AspectValidator _aspectValidator;
    private readonly ConstraintValidator _constraintValidator;
    private readonly EntityValidator _entityValidator;
    private readonly EntityTypeValidator _entityTypeValidator;
    private readonly RelationValidator _relationValidator;
    private readonly StateValidator _stateValidator;
    private readonly StateMachineValidator _stateMachineValidator;
    private readonly TransitionValidator _transitionValidator;

    public MetaStateValidator(AspectValidator aspectValidator, ConstraintValidator constraintValidator, EntityValidator entityValidator, EntityTypeValidator entityTypeValidator, RelationValidator relationValidator, StateValidator stateValidator, StateMachineValidator stateMachineValidator, TransitionValidator transitionValidator)
    {
        _aspectValidator = aspectValidator;
        _constraintValidator = constraintValidator;
        _entityValidator = entityValidator;
        _entityTypeValidator = entityTypeValidator;
        _relationValidator = relationValidator;
        _stateValidator = stateValidator;
        _stateMachineValidator = stateMachineValidator;
        _transitionValidator = transitionValidator;
    }


    private MetaState _system;

    public ValidationErrors Validate(MetaState system)
    {
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

    private void ValidateRelations(MetaState system, ValidationErrors validationErrors)
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

    private void ValidateStateMachines(MetaState system, ValidationErrors validationErrors)
    {
        // Uniqueness of state type names
        var names = system.StateMachines.GroupBy(s => s.Name);
        foreach (var name in names)
        {
            validationErrors.AddIf(name.Count() > 1, new NameNotUnique($"State type name {name.Key} is not unique. {name.Count()} instances found."));
        }
    }

    private void ValidateStates(MetaState system, ValidationErrors validationErrors)
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

    private void CheckForUnresolvableReferences(MetaState system)
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

    private void CheckForNameUniquenessByType(MetaState system)
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
        ArgumentNullExceptionStandard.ThrowIfNull(_system);
        return _aspectValidator.Validate(_system, aspect);
    }

    public object VisitAspectType(AspectType aspectType)
    {
        throw new NotImplementedException();
        //ArgumentNullExceptionStandard.ThrowIfNull(_system);
        //return _aspectTypeValidator.Validate(_system, aspectType);
    }

    public object VisitConstraint(Constraint constraint)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_system);
        return _constraintValidator.Validate(_system, constraint);
    }

    public object VisitEntity(Entity entity)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_system);
        return _entityValidator.Validate(_system, entity);
    }

    public object VisitEntityType(EntityType entityType)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_system);
        return _entityTypeValidator.Validate(_system, entityType);
    }

    public object VisitRelation(Relation relation)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_system);
        return _relationValidator.Validate(_system, relation);
    }

    public object VisitState(State state)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_system);
        return _stateValidator.Validate(_system, state);
    }

    public object VisitStateMachine(StateMachine stateMachine)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_system);
        return _stateMachineValidator.Validate(_system, stateMachine);
    }

    public object VisitTransition(Transition transition)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_system);
        return _transitionValidator.Validate(_system, transition);
    }
}