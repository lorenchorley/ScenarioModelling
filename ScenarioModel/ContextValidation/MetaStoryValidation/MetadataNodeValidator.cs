using ScenarioModelling.ContextValidation.Errors;
using ScenarioModelling.ContextValidation.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.StoryNodes;

namespace ScenarioModelling.ContextValidation.MetaStoryValidation;

[StoryNodeLike<INodeValidator, MetadataNode>]
public class MetadataNodeValidator : INodeValidator<MetadataNode>
{
    public ValidationErrors Validate(System system, MetaStory MetaStory, MetadataNode node)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}