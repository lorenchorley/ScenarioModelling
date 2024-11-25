using ScenarioModel.ContextValidation.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.Validation;

namespace ScenarioModel.ContextValidation.SystemValidation;

[ObjectLike<IObjectValidator, Relation>]
public class RelationValidator : IObjectValidator<Relation>
{
    public ValidationErrors Validate(System system, Relation relation)
    {
        ValidationErrors validationErrors = new();



        return validationErrors;
    }
}