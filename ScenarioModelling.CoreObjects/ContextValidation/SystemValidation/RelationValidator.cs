using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.SystemObjects;

namespace ScenarioModelling.CoreObjects.ContextValidation.SystemValidation;

[SystemObjectLike<IObjectValidator, Relation>]
public class RelationValidator : IObjectValidator<Relation>
{
    public ValidationErrors Validate(MetaState system, Relation relation)
    {
        ValidationErrors validationErrors = new();



        return validationErrors;
    }
}