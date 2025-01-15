using ScenarioModelling.ContextValidation.Errors;
using ScenarioModelling.ContextValidation.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.ScenarioNodes;

namespace ScenarioModelling.ContextValidation.ScenarioValidation;

[NodeLike<INodeValidator, JumpNode>]
public class JumpNodeValidator : INodeValidator<JumpNode>
{
    public ValidationErrors Validate(System system, MetaStory scenario, JumpNode node)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}