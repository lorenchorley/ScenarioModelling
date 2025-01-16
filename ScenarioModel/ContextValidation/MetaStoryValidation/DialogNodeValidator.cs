using ScenarioModelling.ContextValidation.Errors;
using ScenarioModelling.ContextValidation.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.StoryNodes;

namespace ScenarioModelling.ContextValidation.MetaStoryValidation;

[StoryNodeLike<INodeValidator, DialogNode>]
public class DialogNodeValidator : INodeValidator<DialogNode>
{
    public ValidationErrors Validate(System system, MetaStory MetaStory, DialogNode node)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}