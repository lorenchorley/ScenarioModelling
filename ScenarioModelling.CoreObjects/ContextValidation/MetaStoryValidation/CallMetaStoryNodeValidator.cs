using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.Tools.Collections.Graph;

namespace ScenarioModelling.CoreObjects.ContextValidation.MetaStoryValidation;

[StoryNodeLike<INodeValidator, CallMetaStoryNode>]
public class CallMetaStoryNodeValidator : INodeValidator<CallMetaStoryNode>
{
    public ValidationErrors Validate(MetaState system, MetaStory MetaStory, CallMetaStoryNode node)
    {
        ValidationErrors validationErrors = new();

        // TODO Check if the secondary meta story exists, need to accept the list of all meta stories here though

        return validationErrors;
    }

    //private bool IsNodeName(SemiLinearSubGraph<IStoryNode> graph, string name)
    //{
    //    return graph.NodeSequence.Any(s => s.Name.IsEqv(name));
    //}
}