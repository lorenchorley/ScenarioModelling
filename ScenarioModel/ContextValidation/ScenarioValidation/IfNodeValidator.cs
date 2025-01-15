using ScenarioModelling.ContextValidation.Errors;
using ScenarioModelling.ContextValidation.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.ScenarioNodes;

namespace ScenarioModelling.ContextValidation.ScenarioValidation;

[NodeLike<INodeValidator, IfNode>]
public class IfNodeValidator : INodeValidator<IfNode>
{
    public ValidationErrors Validate(System system, MetaStory scenario, IfNode node)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}