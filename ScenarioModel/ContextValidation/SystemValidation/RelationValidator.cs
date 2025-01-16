using ScenarioModelling.ContextValidation.Errors;
using ScenarioModelling.ContextValidation.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects;

namespace ScenarioModelling.ContextValidation.SystemValidation;

[SystemObjectLike<IObjectValidator, Relation>]
public class RelationValidator : IObjectValidator<Relation>
{
    public ValidationErrors Validate(System system, Relation relation)
    {
        ValidationErrors validationErrors = new();



        return validationErrors;
    }
}