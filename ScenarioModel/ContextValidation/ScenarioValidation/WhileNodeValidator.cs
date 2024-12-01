using ScenarioModel.ContextValidation.Errors;
using ScenarioModel.ContextValidation.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;

namespace ScenarioModel.ContextValidation.ScenarioValidation;

[NodeLike<INodeValidator, WhileNode>]
public class WhileNodeValidator : INodeValidator<WhileNode>
{
    public ValidationErrors Validate(System system, Scenario scenario, WhileNode node)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}