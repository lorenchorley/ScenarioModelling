using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects;

namespace ScenarioModelling.CoreObjects.ContextValidation.MetaStateValidation;

[MetaStateObjectLike<IObjectValidator, Relation>]
public class RelationValidator : IObjectValidator<Relation>
{
    public ValidationErrors Validate(MetaState system, Relation relation)
    {
        ValidationErrors validationErrors = new();



        return validationErrors;
    }
}