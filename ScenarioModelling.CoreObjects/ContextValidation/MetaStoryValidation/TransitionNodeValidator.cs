using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.StoryNodes;

namespace ScenarioModelling.CoreObjects.ContextValidation.MetaStoryValidation;

[StoryNodeLike<INodeValidator, TransitionNode>]
public class TransitionNodeValidator : INodeValidator<TransitionNode>
{
    public ValidationErrors Validate(MetaState system, MetaStory MetaStory, TransitionNode node)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}