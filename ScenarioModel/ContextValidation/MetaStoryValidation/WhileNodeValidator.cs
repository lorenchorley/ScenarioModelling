using ScenarioModelling.ContextValidation.Errors;
using ScenarioModelling.ContextValidation.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.StoryNodes;

namespace ScenarioModelling.ContextValidation.MetaStoryValidation;

[StoryNodeLike<INodeValidator, WhileNode>]
public class WhileNodeValidator : INodeValidator<WhileNode>
{
    public ValidationErrors Validate(System system, MetaStory MetaStory, WhileNode node)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}