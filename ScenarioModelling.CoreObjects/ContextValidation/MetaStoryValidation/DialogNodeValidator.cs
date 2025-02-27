using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.MetaStoryNodes;

namespace ScenarioModelling.CoreObjects.ContextValidation.MetaStoryValidation;

[StoryNodeLike<INodeValidator, DialogNode>]
public class DialogNodeValidator : INodeValidator<DialogNode>
{
    public ValidationErrors Validate(MetaState system, MetaStory MetaStory, DialogNode node)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}