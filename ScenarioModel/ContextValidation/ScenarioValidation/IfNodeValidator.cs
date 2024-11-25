using ScenarioModel.ContextValidation.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Validation;

namespace ScenarioModel.ContextValidation.ScenarioValidation;

[NodeLike<INodeValidator, IfNode>]
public class IfNodeValidator : INodeValidator<IfNode>
{
    public ValidationErrors Validate(System system, Scenario scenario, IfNode node)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}