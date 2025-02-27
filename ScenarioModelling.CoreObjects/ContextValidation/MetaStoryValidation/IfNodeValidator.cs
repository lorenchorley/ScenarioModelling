using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.MetaStoryNodes;

namespace ScenarioModelling.CoreObjects.ContextValidation.MetaStoryValidation;

[StoryNodeLike<INodeValidator, IfNode>]
public class IfNodeValidator : INodeValidator<IfNode>
{
    public ValidationErrors Validate(MetaState system, MetaStory MetaStory, IfNode node)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}