using ScenarioModelling.ContextValidation.Errors;
using ScenarioModelling.ContextValidation.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.StoryNodes;

namespace ScenarioModelling.ContextValidation.MetaStoryValidation;

[StoryNodeLike<INodeValidator, IfNode>]
public class IfNodeValidator : INodeValidator<IfNode>
{
    public ValidationErrors Validate(System system, MetaStory MetaStory, IfNode node)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}