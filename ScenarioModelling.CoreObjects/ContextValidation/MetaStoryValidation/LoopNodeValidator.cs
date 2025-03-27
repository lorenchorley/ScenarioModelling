using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.MetaStoryNodes;

namespace ScenarioModelling.CoreObjects.ContextValidation.MetaStoryValidation;

[StoryNodeLike<INodeValidator, LoopNode>]
public class LoopNodeValidator : INodeValidator<LoopNode>
{
    public ValidationErrors Validate(MetaState system, MetaStory MetaStory, LoopNode node)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}