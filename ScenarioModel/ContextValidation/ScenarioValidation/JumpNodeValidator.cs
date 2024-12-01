using ScenarioModel.ContextValidation.Errors;
using ScenarioModel.ContextValidation.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;

namespace ScenarioModel.ContextValidation.ScenarioValidation;

[NodeLike<INodeValidator, JumpNode>]
public class JumpNodeValidator : INodeValidator<JumpNode>
{
    public ValidationErrors Validate(System system, Scenario scenario, JumpNode node)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}