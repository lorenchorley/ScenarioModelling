using ScenarioModel.ContextValidation.Errors;
using ScenarioModel.ContextValidation.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;

namespace ScenarioModel.ContextValidation.ScenarioValidation;

[NodeLike<INodeValidator, TransitionNode>]
public class TransitionNodeValidator : INodeValidator<TransitionNode>
{
    public ValidationErrors Validate(System system, Scenario scenario, TransitionNode node)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}