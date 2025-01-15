using ScenarioModelling.ContextValidation.Errors;
using ScenarioModelling.ContextValidation.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.ScenarioNodes;

namespace ScenarioModelling.ContextValidation.ScenarioValidation;

[NodeLike<INodeValidator, WhileNode>]
public class WhileNodeValidator : INodeValidator<WhileNode>
{
    public ValidationErrors Validate(System system, MetaStory scenario, WhileNode node)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}