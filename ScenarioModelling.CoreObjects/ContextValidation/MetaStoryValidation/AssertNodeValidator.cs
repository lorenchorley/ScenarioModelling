using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.Tools.Collections.Graph;

namespace ScenarioModelling.CoreObjects.ContextValidation.MetaStoryValidation;

[StoryNodeLike<INodeValidator, AssertNode>]
public class AssertNodeValidator : INodeValidator<AssertNode>
{
    public ValidationErrors Validate(MetaState system, MetaStory MetaStory, AssertNode node)
    {
        ValidationErrors validationErrors = new();

        // TODO Check if the secondary meta story exists, need to accept the list of all meta stories here though

        return validationErrors;
    }
}