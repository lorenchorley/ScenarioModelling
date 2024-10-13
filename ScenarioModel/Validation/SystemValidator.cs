using ScenarioModel.SystemObjects.Relations;

namespace ScenarioModel.Validation;

public class SystemValidator : IValidator<System>
{
    private EntityValidator _entityValidator = new();
    private RelationValidator _relationValidator = new();
    private StateValidator _stateValidator = new();

    public ValidationErrors Validate(System system)
    {
        ValidationErrors validationErrors = new();

        // All names are unique
        // Everything is valid

        ValidateEntityTypes(system, validationErrors);
        ValidateStateTypes(system, validationErrors);
        ValidateStates(system, validationErrors);
        ValidateRelations(system, validationErrors);
        ValidateConstraints(system, validationErrors);

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
            _relationValidator.Validate(relation);
        }
    }

    private void ValidateStateTypes(System system, ValidationErrors validationErrors)
    {
        // Uniqueness of state type names
        var names = system.StateMachines.GroupBy(s => s.Name);
        foreach (var name in names)
        {
            validationErrors.AddIf(name.Count() > 1, new NameNotUnique($"State type name {name.Key} is not unique. {name.Count()} instances found."));
        }

        foreach (var stateType in system.StateMachines)
        {
            _stateValidator.ValidateType(stateType);
        }
    }

    private void ValidateStates(System system, ValidationErrors validationErrors)
    {
        // Uniqueness of state names
        var names = system.AllStates.GroupBy(s => s.Name);
        foreach (var name in names)
        {
            validationErrors.AddIf(name.Count() > 1, new NameNotUnique($"State name {name.Key} is not unique. {name.Count()} instances found."));
        }

        foreach (var state in system.AllStates)
        {
            _stateValidator.Validate(state);
        }
    }

    private void ValidateEntityTypes(System system, ValidationErrors validationErrors)
    {
        foreach (var entityType in system.EntityTypes)
        {
            _entityValidator.ValidateType(entityType);
        }
    }

    private void ValidateConstraints(System system, ValidationErrors validationErrors)
    {
        ConstraintValidator validator = new(system);

        foreach (var constraint in system.Constraints)
        {
            validationErrors.Incorporate(validator.Validate(constraint));
        }
    }
}