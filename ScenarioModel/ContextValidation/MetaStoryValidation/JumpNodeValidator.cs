using ScenarioModelling.ContextValidation.Errors;
using ScenarioModelling.ContextValidation.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.StoryNodes;

namespace ScenarioModelling.ContextValidation.MetaStoryValidation;

[StoryNodeLike<INodeValidator, JumpNode>]
public class JumpNodeValidator : INodeValidator<JumpNode>
{
    public ValidationErrors Validate(System system, MetaStory MetaStory, JumpNode node)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}