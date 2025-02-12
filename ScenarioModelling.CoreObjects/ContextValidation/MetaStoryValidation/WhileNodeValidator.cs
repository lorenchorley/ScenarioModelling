using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.StoryNodes;

namespace ScenarioModelling.CoreObjects.ContextValidation.MetaStoryValidation;

[StoryNodeLike<INodeValidator, WhileNode>]
public class WhileNodeValidator : INodeValidator<WhileNode>
{
    public ValidationErrors Validate(MetaState system, MetaStory MetaStory, WhileNode node)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}