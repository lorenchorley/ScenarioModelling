using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
namespace ScenarioModelling.CoreObjects.ContextValidation.MetaStoryValidation;

[StoryNodeLike<INodeValidator, MetadataNode>]
public class MetadataNodeValidator : INodeValidator<MetadataNode>
{
    public ValidationErrors Validate(MetaState system, MetaStory MetaStory, MetadataNode node)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}