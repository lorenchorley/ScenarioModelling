using ScenarioModelling.ContextValidation.Errors;
using ScenarioModelling.ContextValidation.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.ScenarioNodes;

namespace ScenarioModelling.ContextValidation.ScenarioValidation;

[NodeLike<INodeValidator, TransitionNode>]
public class TransitionNodeValidator : INodeValidator<TransitionNode>
{
    public ValidationErrors Validate(System system, MetaStory scenario, TransitionNode node)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}