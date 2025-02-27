using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.MetaStoryNodes;

namespace ScenarioModelling.CoreObjects.ContextValidation.MetaStoryValidation;

[StoryNodeLike<INodeValidator, JumpNode>]
public class JumpNodeValidator : INodeValidator<JumpNode>
{
    public ValidationErrors Validate(MetaState system, MetaStory MetaStory, JumpNode node)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}