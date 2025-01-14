using ScenarioModel.ContextValidation.Errors;
using ScenarioModel.ContextValidation.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;

namespace ScenarioModel.ContextValidation.ScenarioValidation;

[NodeLike<INodeValidator, DialogNode>]
public class DialogNodeValidator : INodeValidator<DialogNode>
{
    public ValidationErrors Validate(System system, MetaStory scenario, DialogNode node)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}