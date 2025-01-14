using ScenarioModel.ContextValidation.Errors;
using ScenarioModel.ContextValidation.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;

namespace ScenarioModel.ContextValidation.ScenarioValidation;

[NodeLike<INodeValidator, IfNode>]
public class IfNodeValidator : INodeValidator<IfNode>
{
    public ValidationErrors Validate(System system, MetaStory scenario, IfNode node)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}