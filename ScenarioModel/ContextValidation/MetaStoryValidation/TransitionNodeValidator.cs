using ScenarioModelling.ContextValidation.Errors;
using ScenarioModelling.ContextValidation.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.StoryNodes;

namespace ScenarioModelling.ContextValidation.MetaStoryValidation;

[StoryNodeLike<INodeValidator, TransitionNode>]
public class TransitionNodeValidator : INodeValidator<TransitionNode>
{
    public ValidationErrors Validate(System system, MetaStory MetaStory, TransitionNode node)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}