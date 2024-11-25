using ScenarioModel.ContextValidation.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Validation;

namespace ScenarioModel.ContextValidation.ScenarioValidation;

[NodeLike<INodeValidator, DialogNode>]
public class DialogNodeValidator : INodeValidator<DialogNode>
{
    public ValidationErrors Validate(System system, Scenario scenario, DialogNode node)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}